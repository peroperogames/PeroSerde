/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: Extensions
 */

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

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
}