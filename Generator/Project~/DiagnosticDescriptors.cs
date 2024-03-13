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

    #region 构造函数问题

    /// <summary>有多个构造函数, 并且都没带[SerdeJsonConstructor]标签</summary>
    public static readonly DiagnosticDescriptor multipleCtorWithoutAttribute = new(
        id: "SERDE006",
        title: "Require [SerdeJsonConstructor] when exists multiple constructors",
        messageFormat: "The SerdeSerializable object '{0}' must annotate with [SerdeJsonConstructor] when exists multiple constructors",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>有多个构造函数带有[SerdeJsonConstructor]标签</summary>
    public static readonly DiagnosticDescriptor multipleCtorWithAttribute = new(
        id: "SERDE007",
        title: "[SerdeJsonConstructor] exists in multiple constructors",
        messageFormat: "Multiple [SerdeJsonConstructor] exists in '{0}' but allows only single ctor",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>构造函数参数中含有未知参数(没在members里)</summary>
    public static readonly DiagnosticDescriptor ctorHasNoMatchedParameter = new(
        id: "SERDE008",
        title: "Constructor has no matched parameter",
        messageFormat: "The serde serializable object '{0}' constructor's parameter '{1}' must match a serialized member name(case-insensitive)",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>只读Field必须是构造函数参数.</summary>
    public static readonly DiagnosticDescriptor readonlyFieldMustBeCtorMember = new(
        id: "SERDE009",
        title: "Readonly field must be constructor member",
        messageFormat: "Type '{0}''s readonly field '{1}' must be constructor member",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    #endregion
}