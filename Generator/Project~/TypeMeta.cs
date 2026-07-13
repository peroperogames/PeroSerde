/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/09/09 13:09:49
 */

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace PeroTools2.Serde.Generator;

/// <summary>
/// Json 序列化/反序列化所需的类型元数据
/// </summary>
public struct TypeMeta
{
    /// <summary>
    /// 当前类型符号信息
    /// </summary>
    public readonly ITypeSymbol symbol;

    /// <summary>
    /// 可序列化/反序列化的成员
    /// </summary>
    public readonly MemberMeta[] members;

    /// <summary>
    /// 若为 true，则在序列化时忽略值为 null 的成员
    /// </summary>
    public readonly bool ignoreNullMembers;

    public TypeMeta(ITypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        symbol = type;
        members = GetMemberMetas(type, referenceSymbols);
        ignoreNullMembers = GetIgnoreNullMembers(type, referenceSymbols);
    }

    /// <summary>
    /// 读取 <c>SerdeJsonSerializableAttribute</c> 上的 <c>ignoreNullMembers</c> 构造参数（第二个参数）
    /// </summary>
    private static bool GetIgnoreNullMembers(ITypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        if (type.TryGetAttribute(referenceSymbols.serdeJsonSerializableAttribute, out var attr)
            && attr!.ConstructorArguments.Length > 1
            && attr.ConstructorArguments[1].Value is bool ignoreNull)
        {
            return ignoreNull;
        }
        return false;
    }

    /// <summary>
    /// 获取所有可序列化/反序列化成员的信息
    /// </summary>
    private MemberMeta[] GetMemberMetas(ITypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        var result = new List<MemberMeta>();
        var members = type.GetAllMembers();
        foreach (var member in members)
        {
            // 除了 Enum 不考虑 Ignore 和静态成员
            if (type.TypeKind != TypeKind.Enum && (member.ContainsAttribute(referenceSymbols.serdeJsonIgnoreMemberAttribute) || member.IsStatic)) continue;

            // 只考虑非只读字段和包含 Set 方法的属性
            var (memberType, formatName) = member switch
            {
                IPropertySymbol { SetMethod: not null } property => (property.Type, property.Name),
                IFieldSymbol { IsReadOnly: false } field => (field.Type, field.Name),
                _ => (null, null)
            };

            // 跳过私有属性的字段
            if (memberType == null || Regex.IsMatch(formatName, "<.+>k__BackingField")) continue;

            var formatter = (ITypeSymbol) null;
            var memberKind = GetMemberKind(memberType, referenceSymbols);
            if (member.TryGetAttribute(referenceSymbols.serdeJsonMemberAttribute, out var memberAttr))
            {
                formatName = memberAttr!.ConstructorArguments.FirstOrDefault().Value as string ?? member.Name;
            }
            if (member.TryGetAttribute(referenceSymbols.serdeJsonFormatterAttribute, out var formatterAttr))
            {
                formatter = formatterAttr!.ConstructorArguments.First().Value as ITypeSymbol;
            }
            result.Add(new MemberMeta
            {
                type = memberType,
                name = member.Name,
                kind = memberKind,
                formatName = formatName,
                formatter = formatter
            });
        }
        return result.ToArray();
    }

    /// <summary>
    /// 根据类型返回实际类型
    /// </summary>
    public static BuiltinKind GetMemberKind(ITypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        if (type is IArrayTypeSymbol)
        {
            return BuiltinKind.Array;
        }
        if (type.IsGenericTypeOf(referenceSymbols.dictionaryType))
        {
            return BuiltinKind.Dictionary;
        }
        if (type.IsGenericTypeOf(referenceSymbols.lazyType))
        {
            return BuiltinKind.Lazy;
        }
        if (type.IsGenericTypeOf(referenceSymbols.keyValuePairType))
        {
            return BuiltinKind.KeyValuePair;
        }
        if (type.IsGenericTypeOf(referenceSymbols.nullableType) || type.ToDisplayString().Last() == '?')
        {
            return BuiltinKind.Nullable;
        }
        if (type.ContainsInterface(referenceSymbols.iCollectionInterface))
        {
            return BuiltinKind.Collection;
        }
        if (type.ContainsInterface(referenceSymbols.iReadonlyCollectionInterface))
        {
            return BuiltinKind.ReadonlyCollection;
        }
        if (type.TypeKind == TypeKind.Enum)
        {
            return BuiltinKind.Enum;
        }
        return BuiltinKind.Value;
    }
}

public struct MemberMeta
{
    public ITypeSymbol type;
    public string name;
    public BuiltinKind kind;
    public string formatName;
    public ITypeSymbol? formatter;
    public bool useFormatter => formatter != null || kind != BuiltinKind.Value;

    /// <summary>
    /// 该成员的值是否可能为 null（引用类型或 <c>Nullable&lt;T&gt;</c>）
    /// </summary>
    public bool canBeNull => type is { IsReferenceType: true } || kind == BuiltinKind.Nullable;
}