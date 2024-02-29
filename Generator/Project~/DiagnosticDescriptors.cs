/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: DiagnosticDescriptors
 */

using Microsoft.CodeAnalysis;

namespace PeroTools2.Serde.Generator;

internal static class DiagnosticDescriptors
{
    private const string category = "GenerateSerdeSerializer";

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

    /// <summary>不支持ref struct</summary>
    public static readonly DiagnosticDescriptor refStructNotAllow = new(
        id: "SERDE003",
        title: "ref struct",
        messageFormat: "The SerdeSerializable object '{0}' must not be ref struct",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>不支持序列化抽象类或接口</summary>
    public static readonly DiagnosticDescriptor abstractClassOrInterfaceNotAllow = new(
        id: "SERDE004",
        title: "Type is abstract class or interface",
        messageFormat: "The SerdeSerializable object '{0}' must not be abstract/interface",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>有多个构造函数, 并且都没带[SerdeJsonConstructor]标签</summary>
    public static readonly DiagnosticDescriptor multipleCtorWithoutAttribute = new(
        id: "SERDE005",
        title: "Require [SerdeJsonConstructor] when exists multiple constructors",
        messageFormat: "The SerdeSerializable object '{0}' must annotate with [SerdeJsonConstructor] when exists multiple constructors",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>有多个构造函数带有[SerdeJsonConstructor]标签</summary>
    public static readonly DiagnosticDescriptor multipleCtorWithAttribute = new(
        id: "SERDE006",
        title: "[SerdeJsonConstructor] exists in multiple constructors",
        messageFormat: "Multiple [SerdeJsonConstructor] exists in '{0}' but allows only single ctor",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}