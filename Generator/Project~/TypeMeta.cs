/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: TypeMeta
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

public sealed partial class TypeMeta
{
    public TypeMeta(INamedTypeSymbol symbol, ReferenceSymbols references)
    {
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
        // 查找静态序列化方法
        // if ()
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