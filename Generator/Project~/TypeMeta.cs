/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/09/09 13:09:49
 */

using System.Collections.Generic;
using System.Linq;
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
    public readonly INamedTypeSymbol symbol;

    /// <summary>
    /// 可序列化/反序列化的成员
    /// </summary>
    public readonly MemberMeta[] members;

    public TypeMeta(INamedTypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        symbol = type;
        members = GetMemberMetas(type, referenceSymbols);
    }

    /// <summary>
    /// 获取所有可序列化/反序列化成员的信息
    /// </summary>
    private MemberMeta[] GetMemberMetas(INamedTypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        var result = new List<MemberMeta>();
        foreach (var member in type.GetAllMembers())
        {
            // 不考虑 Ignore 和静态成员
            if (member.ContainsAttribute(referenceSymbols.serdeJsonIgnoreMemberAttribute) || member.IsStatic) continue;
            var memberType = member switch
            {
                IPropertySymbol { SetMethod: not null } property => property.Type,
                IFieldSymbol { IsReadOnly: false } field => field.Type,
                _ => null
            };
            // 只考虑非只读字段和包含 Set 方法的属性
            if (memberType == null) continue;

            var formatName = member.Name;
            var formatter = ((ITypeSymbol, ITypeSymbol)?) null;
            var memberKind = GetMemberKind(memberType, referenceSymbols);
            if (member.TryGetAttribute(referenceSymbols.serdeJsonMemberAttribute, out var memberAttr))
            {
                formatName = memberAttr!.ConstructorArguments.FirstOrDefault().Value as string ?? member.Name;
            }
            if (member.TryGetAttribute(referenceSymbols.serdeJsonFormatterAttribute, out var formatterAttr))
            {
                formatter = memberKind is MemberKind.Dictionary or MemberKind.ReadonlyDictionary
                    ? (formatterAttr!.ConstructorArguments[0].Value as ITypeSymbol, formatterAttr.ConstructorArguments[1].Value as ITypeSymbol)
                    : (formatterAttr!.ConstructorArguments.First().Value as ITypeSymbol, null);
            }
            result.Add(new MemberMeta
            {
                type = memberType,
                name = member.Name,
                kind = memberKind,
                formatName = formatName,
                formatter = formatter,
            });
        }
        return result.ToArray();
    }

    /// <summary>
    /// 根据类型返回实际类型
    /// </summary>
    private MemberKind GetMemberKind(ITypeSymbol type, ReferenceSymbols referenceSymbols)
    {
        if (type.ContainsInterface(referenceSymbols.iReadonlyDictionaryInterface))
        {
            return MemberKind.ReadonlyDictionary;
        }
        if (type.ContainsInterface(referenceSymbols.iDictionaryInterface))
        {
            return MemberKind.Dictionary;
        }
        if (type.ContainsInterface(referenceSymbols.iReadonlyListInterface) || type.TypeKind == TypeKind.Array)
        {
            return MemberKind.ReadonlyList;
        }
        if (type.ContainsInterface(referenceSymbols.iListInterface))
        {
            return MemberKind.List;
        }
        return MemberKind.Value;
    }
}

public struct MemberMeta
{
    public ITypeSymbol type;
    public string name;
    public MemberKind kind;
    public string formatName;
    public (ITypeSymbol, ITypeSymbol)? formatter;
}