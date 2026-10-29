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
/// Type metadata required for Json serialization/deserialization
/// </summary>
public struct TypeMeta
{
    /// <summary>
    /// Symbol information of the current type
    /// </summary>
    public readonly ITypeSymbol symbol;

    /// <summary>
    /// Members that can be serialized/deserialized
    /// </summary>
    public readonly MemberMeta[] members;

    /// <summary>
    /// If true, members whose value is null are ignored during serialization
    /// </summary>
    public readonly bool ignoreNullMembers;

    public TypeMeta(ITypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        symbol = type;
        members = GetMemberMetas(type, referenceSymbols);
        ignoreNullMembers = GetIgnoreNullMembers(type, referenceSymbols);
    }

    /// <summary>
    /// Reads the <c>ignoreNullMembers</c> constructor argument (the second one) of <c>SerdeJsonSerializableAttribute</c>
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
    /// Gets information about all serializable/deserializable members
    /// </summary>
    private MemberMeta[] GetMemberMetas(ITypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        var result = new List<MemberMeta>();
        var members = type.GetAllMembers();
        foreach (var member in members)
        {
            // Ignore attributes and static members are not considered, except for Enum
            if (type.TypeKind != TypeKind.Enum && (member.ContainsAttribute(referenceSymbols.serdeJsonIgnoreMemberAttribute) || member.IsStatic)) continue;

            // Only non-readonly fields and properties with a setter are considered
            var (memberType, formatName) = member switch
            {
                IPropertySymbol { SetMethod: not null } property => (property.Type, property.Name),
                IFieldSymbol { IsReadOnly: false } field => (field.Type, field.Name),
                _ => (null, null)
            };

            // Skip the backing fields of private properties
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
    /// Returns the actual kind for the type
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
    /// Whether the value of this member can be null (reference type or <c>Nullable&lt;T&gt;</c>)
    /// </summary>
    public bool canBeNull => type is { IsReferenceType: true } || kind == BuiltinKind.Nullable;
}