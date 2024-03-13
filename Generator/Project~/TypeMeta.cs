/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: TypeMeta
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

public sealed class TypeMeta
{
    public INamedTypeSymbol symbol { get; }
    public string typeName { get; }
    private readonly ReferenceSymbols m_References;

    // 所有可被序列化的成员
    public MemberMeta[] members { get; }
    public IMethodSymbol? constructor { get; }
    public bool isUseEmptyConstructor => constructor == null || constructor.Parameters.IsEmpty;

    // 在运行构造函数时收集到的诊断. 若不为null则意味着TypeMeta构造失败
    private readonly Diagnostic? m_CtorDiagnostic;

    public static bool TryNew(
        TypeDeclarationSyntax typeSyntax,
        INamedTypeSymbol typeSymbol,
        ReferenceSymbols references,
        [NotNullWhen(true)]
        out TypeMeta? typeMeta,
        [NotNullWhen(false)]
        out Diagnostic? diagnostic
    )
    {
        typeMeta = null;
        if (!PreValidate(typeSymbol, typeSyntax, out diagnostic))
            return false;
        typeMeta = new TypeMeta(typeSymbol, typeSyntax, references);
        if (!PostValidate(typeMeta, typeSyntax, out diagnostic))
            return false;
        return true;
    }

    private TypeMeta(INamedTypeSymbol symbol, TypeDeclarationSyntax typeSyntax, ReferenceSymbols references)
    {
        this.symbol = symbol;
        m_References = references;
        typeName = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        (constructor, m_CtorDiagnostic) = ChooseConstructor(typeSyntax, symbol, references);
        members = symbol.GetAllMembers()
            // 仅包含用户定义的, 非静态的field和property
            .Where(static s => s is (IFieldSymbol or IPropertySymbol) and { IsStatic: false, IsImplicitlyDeclared: false, CanBeReferencedByName: true })
            // 过滤掉仅set的property和indexer
            .Where(static s => !(s is IPropertySymbol p && (p.GetMethod == null || p.IsIndexer)))
            // 根据可访问性和include/ignore标签过滤
            .Where(s =>
            {
                var include = s.ContainsAttribute(references.serdeJsonIncludeMemberAttribute);
                var ignore = s.ContainsAttribute(references.serdeJsonIgnoreMemberAttribute);
                if (ignore) return false;
                if (include) return true;
                return s.DeclaredAccessibility is Accessibility.Public;
            })
            .DistinctBy(static m => m.Name)
            .Select((s, i) => new MemberMeta(s, constructor, references, i))
            .OrderBy(static s => s.order)
            .ToArray();
    }

    private static bool PostValidate(TypeMeta typeMeta, TypeDeclarationSyntax typeSyntax, [NotNullWhen(false)] out Diagnostic? diagnostic)
    {
        if (typeMeta.m_CtorDiagnostic != null)
        {
            diagnostic = typeMeta.m_CtorDiagnostic;
            return false;
        }
        // 判断ctor的每个参数是不是都有对应的member
        if (typeMeta.constructor != null)
        {
            foreach (var parameter in typeMeta.constructor.Parameters)
            {
                var hasMatchingMember = typeMeta.members.Any(member =>
                    member.isConstructorParameter
                    && string.Equals(parameter.Name, member.constructorParameterName, StringComparison.OrdinalIgnoreCase)
                );
                if (!hasMatchingMember)
                {
                    var location =
                        parameter.Locations.FirstOrDefault() ??
                        typeMeta.constructor.Locations.FirstOrDefault() ??
                        typeSyntax.Identifier.GetLocation();
                    diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.ctorHasNoMatchedParameter,
                        location,
                        typeMeta.typeName, parameter.Name
                    );
                    return false;
                }
            }
        }
        // 判断是否有只读Field, 并且不是构造函数参数
        foreach (var member in typeMeta.members)
        {
            if (member.symbol is IFieldSymbol field && field.IsReadOnly && !member.isConstructorParameter)
            {
                diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.readonlyFieldMustBeCtorMember,
                    member.GetLocation(typeSyntax),
                    typeMeta.typeName, member.name
                );
                return false;
            }
        }
        // TODO 不能包含不能序列化的Member
        diagnostic = null;
        return true;
    }

    /// <summary>
    /// 为了节约性能, 能在生成TypeMeta前做的检查放在前面做.
    /// </summary>
    private static bool PreValidate(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax typeSyntax, [NotNullWhen(false)] out Diagnostic? diagnostic)
    {
        if (!typeSyntax.IsPartial())
        {
            diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.mustBePartial,
                typeSyntax.Identifier.GetLocation(),
                typeSymbol.Name
            );
            return false;
        }
        if (typeSyntax.IsNested())
        {
            diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.nestedNotAllow,
                typeSyntax.Identifier.GetLocation(),
                typeSymbol.Name
            );
            return false;
        }
        if (typeSyntax.IsStatic())
        {
            diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.staticClassNotAllow,
                typeSyntax.Identifier.GetLocation(),
                typeSymbol.Name
            );
            return false;
        }
        // ref struct 没法序列化
        if (typeSymbol.IsRefLikeType)
        {
            diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.refStructNotAllow,
                typeSyntax.Identifier.GetLocation(),
                typeSymbol.Name
            );
            return false;
        }
        // abstract class / interface 没法序列化.
        if (typeSymbol.IsAbstract)
        {
            diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.abstractClassOrInterfaceNotAllow,
                typeSyntax.Identifier.GetLocation(),
                typeSymbol.Name
            );
            return false;
        }
        diagnostic = null;
        return true;
    }

    private static (IMethodSymbol? ctor, Diagnostic?) ChooseConstructor(
        TypeDeclarationSyntax typeSyntax, INamedTypeSymbol typeSymbol, ReferenceSymbols references
    )
    {
        var ctors = typeSymbol.InstanceConstructors
            .Where(static ctor => !ctor.IsImplicitlyDeclared)
            .ToArray();
        switch (ctors.Length)
        {
            case 0: // 没有显式声明的ctor
                return (null, null);
            case 1: // 只有一个ctor, 就是你了
                return (ctors[0], null);
            default:
            {
                // 多个ctor的情况, 需要显式指定[SerdeJsonConstructor]标签
                var ctorWithAttrs = ctors
                    .Where(ctor => ctor.ContainsAttribute(references.serdeJsonConstructorAttribute))
                    .ToArray();
                return ctorWithAttrs.Length switch
                {
                    0 => (null, Diagnostic.Create(DiagnosticDescriptors.multipleCtorWithoutAttribute, typeSyntax.GetLocation(), typeSymbol.Name)),
                    1 => (ctorWithAttrs[0], null),
                    _ => (null, Diagnostic.Create(DiagnosticDescriptors.multipleCtorWithAttribute, typeSyntax.GetLocation(), typeSymbol.Name))
                };
            }
        }
    }
}

/*
 * 用户Class/Struct/Record: 1. 类型内的静态序列化方法 2. 查找CustomFormatter(自动生成或者用户手写) 3. Fallback到反射
 * Primitive/String: 内置序列化方法
 * KnownType: 预定义Formatter
 * Enum: Enum快速反射(注意Flags和未定义值), 用户定义的Enum支持使用Attribute自定义字符串值
 * Nullable: NullableWrapper
 * Collection: CollectionWrapper
 *
 * [SerdeSerializable]支持反序列化时使用构造函数, 序列化时自定义字段名, Ignore字段, IgnoreDefault/EmptyString
 * 支持多态序列化(序列化接口/抽象类)
 * (初版不做)支持序列化回调
 * 支持异步序列化
 * 使用[SerdeSerializable]生成的代码更优先. 如果需要使用CustomFormatter则需要取消[SerdeSerializable]标签
 * (初版不做)用户可以在Config中以字典的形式提供本次使用的特殊Formatter
 * Config: Indent, CustomFormatters, DefaultValueBehaviour, NullValueBehaviour, EmptyStringValueBehaviour, EnumValueBehaviour
 * TODO 循环引用?
 * (初版不做)TODO 假定有序的反序列化, 可以提高性能
 * TODO 注意反序列化性能!
 *
 * 需要的Attribute:
 * [SerdeJsonSerializable(NamingConvention)]
 * [SerdeJsonMember(Name, NamingConvention)]
 * [SerdeJsonIgnoreMember]
 * [SerdeJsonConstructor]
 * [SerdeJsonCustomFormatter]
 */

public enum MemberKind
{
    // TODO 多态序列化
    NonSerializable, // object, delegate, [SerdeJsonIgnoreMember]
    SerdeSerializable, // class/struct/record with [SerdeJsonSerializable]
    CustomFormatter, // class/struct/record without [SerdeJsonSerializable] and have [SerdeJsonCustomFormatter]
    FallbackToReflection, // class/struct/record without [SerdeJsonSerializable] and [SerdeJsonCustomFormatter]
    Primitive, // bool, integer, float, char
    String, // string
    KnownType, // KeyValuePair<>, Tuple<>, TimeSpan, DateTime, DateTimeOffset, Guid, Uri, Version, BitArray, Type, ArraySegment<T>, BigInteger, Complex, UnityTypes.
    Enum, // Enum
    Nullable, // Nullable<T>
    Collection, // T[], List<T>, ICollection<T>...
}

public sealed partial class MemberMeta
{
    public ISymbol symbol { get; }
    public string name { get; }
    public int order { get; }
    public ITypeSymbol memberType { get; }

    // public INamedTypeSymbol? customFormatter { get; }
    // public string? customFormatterName { get; }

    public bool isField { get; }
    public bool isProperty { get; }
    public bool isSettable { get; }
    public bool isAssignable { get; }
    public bool isConstructorParameter { get; }
    public string? constructorParameterName { get; }
    public MemberKind kind { get; }

    public MemberMeta(ISymbol symbol, IMethodSymbol? constructor, ReferenceSymbols references, int sequentialOrder)
    {
        this.symbol = symbol;
        name = symbol.Name;
        order = sequentialOrder;
        if (constructor != null)
        {
            isConstructorParameter = constructor.TryGetCtorParam(symbol, out var ctorParamName);
            constructorParameterName = ctorParamName;
        }
        else
        {
            isConstructorParameter = false;
        }
        if (symbol is IFieldSymbol f)
        {
            isField = true;
            isProperty = false;
            isSettable = isAssignable = !f.IsReadOnly;
            memberType = f.Type;
        }
        else if (symbol is IPropertySymbol p)
        {
            isProperty = true;
            isField = false;
            isSettable = !p.IsReadOnly;
            isAssignable = isSettable && p.SetMethod != null && !p.SetMethod.IsInitOnly;
            memberType = p.Type;
        }
        else
        {
            throw new Exception($"member {symbol.Name} is not field or property.");
        }
        // TODO CustomFormatter
        kind = ParseMemberKind(symbol, memberType, references);
    }

    public Location GetLocation(TypeDeclarationSyntax fallback) =>
        symbol.Locations.FirstOrDefault() ?? fallback.Identifier.GetLocation();

    private static MemberKind ParseMemberKind(ISymbol memberSymbol, ITypeSymbol memberType, ReferenceSymbols references)
    {
        if (memberType.SpecialType is SpecialType.System_Object or SpecialType.System_Delegate or SpecialType.System_MulticastDelegate || memberType.TypeKind is TypeKind.Delegate)
        {
            return MemberKind.NonSerializable;
        }
        if (memberType.TypeKind == TypeKind.Enum)
        {
            return MemberKind.Enum;
        }
        if (memberType.SpecialType == SpecialType.System_String)
        {
            return MemberKind.String;
        }
        if (
            memberType.IsUnmanagedType &&
            memberType.SpecialType
                is SpecialType.System_Boolean
                or SpecialType.System_Byte
                or SpecialType.System_SByte
                or SpecialType.System_Int16
                or SpecialType.System_UInt16
                or SpecialType.System_Int32
                or SpecialType.System_UInt32
                or SpecialType.System_Int64
                or SpecialType.System_UInt64
                or SpecialType.System_Char
                or SpecialType.System_Single
                or SpecialType.System_Double
                or SpecialType.System_Decimal
        )
        {
            return MemberKind.Primitive;
        }
        // // TODO 查找静态序列化方法
        // if (memberType)
        // {
        //     
        // }
        return MemberKind.NonSerializable;
    }
}

public sealed partial class MethodMeta
{
    public IMethodSymbol symbol { get; }
    public string name { get; }
    public bool isStatic { get; }

    public MethodMeta(IMethodSymbol symbol)
    {
        this.symbol = symbol;
        name = symbol.Name;
        isStatic = symbol.IsStatic;
    }
}