/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: Extensions
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

internal static class Extensions
{
    private const string private_member_prefix = "_";
    private const string private_member_prefix2 = "m_";

    internal static (TKey, TValue)[] DestructAll<TKey, TValue>(this IDictionary<TKey, TValue> dic)
    {
        var result = new (TKey, TValue)[dic.Count];
        var index = 0;
        foreach (var kv in dic)
        {
            result[index++] = (kv.Key, kv.Value);
        }
        return result;
    }

    /// <summary>
    /// 查找一个成员变量是否在该构造函数中有对应的形参.<br/>
    /// 支持m_和_作为前缀的成员变量.
    /// </summary>
    internal static bool TryGetCtorParam(this IMethodSymbol ctor, ISymbol member, out string? ctorParamName)
    {
        var ctorParam = GetCtorParam(ctor, member.Name);
        ctorParamName = ctorParam?.Name;
        return ctorParam != null;

        static IParameterSymbol? GetCtorParam(IMethodSymbol ctor, string name)
        {
            return ctor.Parameters.FirstOrDefault(p =>
            {
                if (name.Equals(p.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (
                    name.StartsWith(private_member_prefix, StringComparison.Ordinal) &&
                    name.AsSpan()[..private_member_prefix.Length].Equals(p.Name.AsSpan(), StringComparison.OrdinalIgnoreCase)
                )
                {
                    return true;
                }
                if (
                    name.StartsWith(private_member_prefix2, StringComparison.Ordinal) &&
                    name.AsSpan()[..private_member_prefix2.Length].Equals(p.Name.AsSpan(), StringComparison.OrdinalIgnoreCase)
                )
                {
                    return true;
                }
                return false;
            });
        }
    }

    internal static bool IsPartial(this TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

    internal static bool IsStatic(this TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);

    internal static bool IsNested(this TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Parent is TypeDeclarationSyntax;

    internal static bool ContainsAttribute(this ISymbol symbol, INamedTypeSymbol attribute) => symbol.TryGetAttribute(attribute, out _);

    internal static bool TryGetAttribute(this ISymbol symbol, INamedTypeSymbol attribute, out AttributeData? attributeData)
    {
        attributeData = symbol.GetAttributes().FirstOrDefault(att => SymbolEqualityComparer.Default.Equals(att.AttributeClass, attribute));
        return attributeData != null;
    }

    internal static bool TryGetListElementType(this INamedTypeSymbol symbol, out ITypeSymbol elementType)
    {
        var listName = ReferenceSymbols.i_list_type_fullname.Replace("`1", string.Empty);
        var readonlyListName = ReferenceSymbols.i_readonly_list_type_fullname.Replace("`1", string.Empty);
        foreach (var interfaceType in symbol.AllInterfaces)
        {
            if (!interfaceType.IsGenericType) continue;
            var typeStr = Regex.Replace(interfaceType.ToDisplayString(), "<.+>", "");
            if (typeStr != listName && typeStr != readonlyListName) continue;
            elementType = symbol.TypeArguments[0];
            return true;
        }
        elementType = null;
        return false;
    }

    internal static bool TryGetDictionaryKeyValueTypes(this INamedTypeSymbol symbol, out ITypeSymbol keyType, out ITypeSymbol valueType)
    {
        var dicName = ReferenceSymbols.i_dictionary_type_fullname.Replace("`2", string.Empty);
        var readonlyDicName = ReferenceSymbols.i_readonly_dictionary_type_fullname.Replace("`2", string.Empty);
        foreach (var interfaceType in symbol.AllInterfaces)
        {
            if (!interfaceType.IsGenericType) continue;
            var typeStr = Regex.Replace(interfaceType.ToDisplayString(), "<.+>", "");
            if (typeStr != dicName && typeStr != readonlyDicName) continue;
            keyType = symbol.TypeArguments[0];
            valueType = symbol.TypeArguments[1];
            return true;
        }
        keyType = null;
        valueType = null;
        return false;
    }

    internal static bool ContainsInterface(this ITypeSymbol symbol, INamedTypeSymbol interfaceType)
        => Enumerable.Any(symbol.AllInterfaces, item => SymbolEqualityComparer.Default.Equals(item, interfaceType)
                                                        || interfaceType.IsGenericType &&
                                                        Regex.Replace(interfaceType.ToDisplayString(), "<.+>", "") == Regex.Replace(item.ToDisplayString(), "<.+>", ""));

    internal static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol symbol, bool withoutOverride = true)
    {
        if (symbol.BaseType != null)
        {
            foreach (var parentMember in GetAllMembers(symbol.BaseType))
            {
                // 如果不限制override, override成员会被统计多次
                if (!withoutOverride || !parentMember.IsOverride)
                {
                    yield return parentMember;
                }
            }
        }
        foreach (var member in symbol.GetMembers())
        {
            if (!withoutOverride || !member.IsOverride)
            {
                yield return member;
            }
        }
    }

    internal static string GetTypeKind(this ITypeSymbol typeSymbol) => typeSymbol.TypeKind switch
    {
        TypeKind.Enum => "enum",
        TypeKind.Interface => "interface",
        TypeKind.Delegate => "delegate",
        _ => (typeSymbol.IsRecord, typeSymbol.IsValueType) switch
        {
            (false, false) => "class",
            (false, true) => "struct",
            (true, false) => "record",
            (true, true) => "record struct",
        }
    };
}