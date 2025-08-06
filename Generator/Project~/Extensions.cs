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

namespace PeroTools2.Serde.Generator;

internal static class Extensions
{
    internal static bool ContainsAttribute(this ISymbol symbol, INamedTypeSymbol attribute) => symbol.TryGetAttribute(attribute, out _);

    internal static bool TryGetAttribute(this ISymbol symbol, INamedTypeSymbol attribute, out AttributeData? attributeData)
    {
        attributeData = symbol.GetAttributes().FirstOrDefault(att => SymbolEqualityComparer.Default.Equals(att.AttributeClass, attribute));
        return attributeData != null;
    }

    internal static bool TryGetCollectionElementType(this ITypeSymbol symbol, out ITypeSymbol elementType)
    {
        if (symbol is IArrayTypeSymbol arrayType)
        {
            elementType = arrayType.ElementType;
            return true;
        }
        var listName = ReferenceSymbols.i_collection_type_fullname.Replace("`1", string.Empty);
        var readonlyListName = ReferenceSymbols.i_readonly_collection_type_fullname.Replace("`1", string.Empty);
        foreach (var interfaceType in symbol.AllInterfaces)
        {
            if (!interfaceType.IsGenericType) continue;
            var typeStr = Regex.Replace(interfaceType.ToDisplayString(), "<.+>", "");
            if (typeStr != listName && typeStr != readonlyListName) continue;
            elementType = interfaceType.TypeArguments[0];
            return true;
        }
        elementType = null;
        return false;
    }

    internal static string GetGenericString(this INamedTypeSymbol symbol)
        => string.Join(", ", symbol.TypeArguments.Select(static t => t.ToDisplayString()));

    internal static string ToDisplayNameWithoutGeneric(this ITypeSymbol symbol)
        => string.IsNullOrEmpty(symbol.ContainingNamespace?.ToDisplayString())
            ? symbol.Name
            : $"{symbol.ContainingNamespace!.ToDisplayString()}.{symbol.Name}";

    internal static bool ContainsInterface(this ITypeSymbol symbol, INamedTypeSymbol interfaceType)
        => Enumerable.Any(symbol.AllInterfaces, item => SymbolEqualityComparer.Default.Equals(item, interfaceType)
                                                        || interfaceType.IsGenericType
                                                        && Regex.Replace(interfaceType.ToDisplayString(), "<.+>", "") == Regex.Replace(item.ToDisplayString(), "<.+>", ""));

    internal static bool IsGeneric(this ITypeSymbol symbol) => symbol is INamedTypeSymbol { IsGenericType: true };

    internal static bool IsNullable(this ITypeSymbol symbol) => !symbol.IsValueType ||
                                                                symbol is INamedTypeSymbol { IsGenericType: true } namedType &&
                                                                "System.Nullable`1" == Regex.Replace(namedType.ToDisplayString(), "<.+>", "");

    internal static bool IsGenericTypeOf(this ITypeSymbol symbol, INamedTypeSymbol genericType)
        => SymbolEqualityComparer.Default.Equals(symbol, genericType)
           || genericType.IsGenericType
           && Regex.Replace(symbol.ToDisplayString(), "<.+>", "") == Regex.Replace(genericType.ToDisplayString(), "<.+>", "");

    internal static string GetAccessibilityString(this ITypeSymbol type)
    {
        return type.DeclaredAccessibility switch
        {
            Accessibility.NotApplicable => string.Empty,
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "protected",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Public => "public",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol symbol, bool withoutOverride = true)
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
            (true, true) => "record struct"
        }
    };
}