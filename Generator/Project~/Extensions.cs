/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: Extensions
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

public static class Extensions
{
    private const string private_member_prefix = "_";
    private const string private_member_prefix2 = "m_";

    /// <summary>
    /// 查找一个成员变量是否在该构造函数中有对应的形参.<br/>
    /// 支持m_和_作为前缀的成员变量.
    /// </summary>
    public static bool TryGetCtorParam(this IMethodSymbol ctor, ISymbol member, out string? ctorParamName)
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

    public static bool IsPartial(this TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

    public static bool IsStatic(this TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);

    public static bool IsNested(this TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Parent is TypeDeclarationSyntax;

    public static bool ContainsAttribute(this ISymbol symbol, INamedTypeSymbol attrSymbol) =>
        symbol
            .GetAttributes()
            .Any(attrData => SymbolEqualityComparer.Default.Equals(attrData.AttributeClass, attrSymbol));

    public static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol symbol, bool withoutOverride = true)
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

    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
    {
        using var enumerator = source.GetEnumerator();
        var set = new HashSet<TKey>(comparer);
        while (enumerator.MoveNext())
        {
            var element = enumerator.Current;
            var key = keySelector.Invoke(element);
            if (set.Add(key))
            {
                yield return element;
            }
        }
    }
}