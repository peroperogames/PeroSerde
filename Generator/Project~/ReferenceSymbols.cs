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
    internal const string serde_json_namespace = "PeroTools2.Serde.Json";
    internal const string system_collection_generic_namespace = "System.Collections.Generic";

    internal const string i_dictionary_type_name = "IDictionary`2";
    internal const string i_readonly_dictionary_type_name = "IReadOnlyDictionary`2";
    internal const string i_list_type_name = "IList`1";
    internal const string i_readonly_list_type_name = "IReadOnlyList`1";

    internal const string i_dictionary_type_fullname = $"{system_collection_generic_namespace}.{i_dictionary_type_name}";
    internal const string i_readonly_dictionary_type_fullname = $"{system_collection_generic_namespace}.{i_readonly_dictionary_type_name}";
    internal const string i_list_type_fullname = $"{system_collection_generic_namespace}.{i_list_type_name}";
    internal const string i_readonly_list_type_fullname = $"{system_collection_generic_namespace}.{i_readonly_list_type_name}";

    internal const string serde_json_serializable_attr_name = "SerdeJsonSerializableAttribute";
    internal const string serde_json_member_attr_name = "SerdeJsonMemberAttribute";
    internal const string serde_json_ignore_member_attr_name = "SerdeJsonIgnoreMemberAttribute";
    internal const string serde_json_formatter_attr_name = "SerdeJsonFormatterAttribute";

    internal const string serde_json_serializable_attr_fullname = $"{serde_json_namespace}.{serde_json_serializable_attr_name}";
    internal const string serde_json_member_attr_fullname = $"{serde_json_namespace}.{serde_json_member_attr_name}";
    internal const string serde_json_ignore_member_attr_fullname = $"{serde_json_namespace}.{serde_json_ignore_member_attr_name}";
    internal const string serde_json_formatter_attr_fullname = $"{serde_json_namespace}.{serde_json_formatter_attr_name}";

    private readonly Compilation m_Compilation;
    public WellKnownTypes wellKnownTypes { get; }

    // Serde Json Attributes
    public INamedTypeSymbol serdeJsonSerializableAttribute { get; }
    public INamedTypeSymbol serdeJsonMemberAttribute { get; }
    public INamedTypeSymbol serdeJsonIgnoreMemberAttribute { get; }
    public INamedTypeSymbol serdeJsonFormatterAttribute { get; }
    public INamedTypeSymbol iDictionaryInterface { get; }
    public INamedTypeSymbol iReadonlyDictionaryInterface { get; }
    public INamedTypeSymbol iListInterface { get; }
    public INamedTypeSymbol iReadonlyListInterface { get; }

    public ReferenceSymbols(Compilation compilation)
    {
        m_Compilation = compilation;
        wellKnownTypes = new WellKnownTypes(this);

        serdeJsonSerializableAttribute = GetTypeByMetaDataName(serde_json_serializable_attr_fullname);
        serdeJsonMemberAttribute = GetTypeByMetaDataName(serde_json_member_attr_fullname);
        serdeJsonIgnoreMemberAttribute = GetTypeByMetaDataName(serde_json_ignore_member_attr_fullname);
        serdeJsonFormatterAttribute = GetTypeByMetaDataName(serde_json_formatter_attr_fullname);
        iDictionaryInterface = GetTypeByMetaDataName(i_dictionary_type_fullname);
        iReadonlyDictionaryInterface = GetTypeByMetaDataName(i_readonly_dictionary_type_fullname);
        iListInterface = GetTypeByMetaDataName(i_list_type_fullname);
        iReadonlyListInterface = GetTypeByMetaDataName(i_readonly_list_type_fullname);
    }

    public INamedTypeSymbol GetTypeByMetaDataName(string metaDataName)
        => m_Compilation.GetTypeByMetadataName(metaDataName) ?? throw new InvalidOperationException($"Type {metaDataName} is not found in compilation.");
}

public sealed class WellKnownTypes
{
    private readonly ReferenceSymbols m_Parent;

    public WellKnownTypes(ReferenceSymbols parent)
    {
        m_Parent = parent;
    }
}