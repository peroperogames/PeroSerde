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

    public static readonly DiagnosticDescriptor mustBePartial = new(
        id: "SERDE001",
        title: "Serde serializable object must be partial",
        messageFormat: "The SerdeSerializable object '{0}' must be partial",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor nestedNotAllow = new(
        id: "SERDE002",
        title: "Serde serializable object must not be nested type",
        messageFormat: "The SerdeSerializable object '{0}' must not be nested type",
        category: category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}