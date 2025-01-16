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
    public static DiagnosticDescriptor CreateErrorDescriptor(Exception exception) => new DiagnosticDescriptor(
        id: "SERDE000",
        title: "Generate error.",
        messageFormat: exception.ToString(),
        category: DiagnosticDescriptors.category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    #region 类型约束

    /// <summary>需要做代码生成的类必须是partial的</summary>
    public static readonly DiagnosticDescriptor mustBePartial = new(
        id: "SERDE001",
        title: "Serde serializable object must be partial",
        messageFormat: "The SerdeSerializable object '{0}' must be partial",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>不支持嵌套类</summary>
    public static readonly DiagnosticDescriptor nestedNotAllow = new(
        id: "SERDE002",
        title: "Type is nested type",
        messageFormat: "The SerdeSerializable object '{0}' must not be nested type",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>不支持static类</summary>
    public static readonly DiagnosticDescriptor staticClassNotAllow = new(
        id: "SERDE003",
        title: "Type is static class",
        messageFormat: "The SerdeSerializable object '{0}' must not be static class",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>不支持ref struct</summary>
    public static readonly DiagnosticDescriptor refStructNotAllow = new(
        id: "SERDE004",
        title: "ref struct",
        messageFormat: "The SerdeSerializable object '{0}' must not be ref struct",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>不支持序列化抽象类或接口</summary>
    public static readonly DiagnosticDescriptor abstractClassOrInterfaceNotAllow = new(
        id: "SERDE005",
        title: "Type is abstract class or interface",
        messageFormat: "The SerdeSerializable object '{0}' must not be abstract/interface",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    #endregion
}