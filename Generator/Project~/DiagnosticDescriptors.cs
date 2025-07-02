/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: DiagnosticDescriptors
 */

using System;
using Microsoft.CodeAnalysis;

namespace PeroTools2.Serde.Generator;

internal static class DiagnosticDescriptors
{
    internal const string category = "GenerateSerdeSerializer";

    /// <summary>
    /// 生成器异常.
    /// </summary>
    public static DiagnosticDescriptor CreateErrorDescriptor(Exception exception) => new(
        "SERDE000",
        "Generate error.",
        exception.ToString(),
        category,
        DiagnosticSeverity.Error,
        true
    );

    #region 类型约束

    /// <summary>需要做代码生成的类必须是partial的</summary>
    public static readonly DiagnosticDescriptor needEmptyCtor = new(
        "SERDE001",
        "Needed empty constructor",
        "The SerdeSerializable object '{0}' must have a public empty constructor",
        category,
        DiagnosticSeverity.Error,
        true
    );

    /// <summary>不支持嵌套类</summary>
    public static readonly DiagnosticDescriptor nestedNotAllow = new(
        "SERDE002",
        "Type is nested type",
        "The SerdeSerializable object '{0}' must not be nested type",
        category,
        DiagnosticSeverity.Error,
        true
    );

    /// <summary>不支持static类</summary>
    public static readonly DiagnosticDescriptor staticClassNotAllow = new(
        "SERDE003",
        "Type is static class",
        "The SerdeSerializable object '{0}' must not be static class",
        category,
        DiagnosticSeverity.Error,
        true
    );

    /// <summary>不支持ref struct</summary>
    public static readonly DiagnosticDescriptor refStructNotAllow = new(
        "SERDE004",
        "ref struct",
        "The SerdeSerializable object '{0}' must not be ref struct",
        category,
        DiagnosticSeverity.Error,
        true
    );

    /// <summary>不支持序列化抽象类或接口</summary>
    public static readonly DiagnosticDescriptor abstractClassOrInterfaceNotAllow = new(
        "SERDE005",
        "Type is abstract class or interface",
        "The SerdeSerializable object '{0}' must not be abstract/interface",
        category,
        DiagnosticSeverity.Error,
        true
    );

    #endregion
}