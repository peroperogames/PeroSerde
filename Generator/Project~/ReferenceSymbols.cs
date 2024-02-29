/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/28
 * Description: ReferenceSymbols
 */

using System;
using Microsoft.CodeAnalysis;

namespace PeroTools2.Serde.Generator;

public sealed class ReferenceSymbols
{
    private readonly Compilation m_Compilation;
    public WellKnownTypes wellKnownTypes { get; }

    // Serde Json Attributes
    public INamedTypeSymbol serdeJsonSerializableAttribute { get; }
    public INamedTypeSymbol serdeJsonMemberAttribute { get; }
    public INamedTypeSymbol serdeJsonIncludeMemberAttribute { get; }
    public INamedTypeSymbol serdeJsonIgnoreMemberAttribute { get; }
    public INamedTypeSymbol serdeJsonConstructorAttribute { get; }
    public INamedTypeSymbol serdeJsonCustomFormatterAttribute { get; }

    public ReferenceSymbols(Compilation compilation)
    {
        m_Compilation = compilation;
        wellKnownTypes = new WellKnownTypes(this);

        serdeJsonSerializableAttribute = GetTypeByMetaDataName(SerdeJsonGenerator.serde_json_serializable_attr_fullname);
        serdeJsonMemberAttribute = GetTypeByMetaDataName(SerdeJsonGenerator.serde_json_member_attr_fullname);
        serdeJsonIncludeMemberAttribute = GetTypeByMetaDataName(SerdeJsonGenerator.serde_json_include_member_attr_fullname);
        serdeJsonIgnoreMemberAttribute = GetTypeByMetaDataName(SerdeJsonGenerator.serde_json_ignore_member_attr_fullname);
        serdeJsonConstructorAttribute = GetTypeByMetaDataName(SerdeJsonGenerator.serde_json_constructor_attr_fullname);
        serdeJsonCustomFormatterAttribute = GetTypeByMetaDataName(SerdeJsonGenerator.serde_json_custom_formatter_attr_fullname);
    }

    private INamedTypeSymbol GetTypeByMetaDataName(string metaDataName)
    {
        var symbol = m_Compilation.GetTypeByMetadataName(metaDataName);
        if (symbol == null)
        {
            throw new InvalidOperationException($"Type {metaDataName} is not found in compilation.");
        }
        return symbol;
    }
}

public sealed class WellKnownTypes
{
    private readonly ReferenceSymbols m_Parent;

    public WellKnownTypes(ReferenceSymbols parent)
    {
        m_Parent = parent;
    }
}