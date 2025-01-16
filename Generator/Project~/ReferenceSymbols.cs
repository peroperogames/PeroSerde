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
    private const string serde_namespace = "PeroTools2.Serde";
    private const string serde_json_namespace = $"{serde_namespace}.Json";
    private const string system_collection_generic_namespace = "System.Collections.Generic";
    private const string system_namespace = "System";
    private const string unity_engine_namespace = "UnityEngine";

    private const string i_collection_type_name = "ICollection`1";
    private const string i_readonly_collection_type_name = "IReadOnlyCollection`1";

    private const string lazy_type_name = "Lazy`1";
    private const string nuallable_type_name = "Nullable`1";
    private const string key_value_pair_type_name = "KeyValuePair`2";

    private const string flags_attr_name = "FlagsAttribute";
    private const string serde_enum_formatter_attr_name = "SerdeJsonEnumFormatterAttribute";
    private const string serde_json_serializable_attr_name = "SerdeJsonSerializableAttribute";
    private const string serde_json_member_attr_name = "SerdeJsonMemberAttribute";
    private const string serde_json_ignore_member_attr_name = "SerdeJsonIgnoreMemberAttribute";
    private const string serde_json_formatter_attr_name = "SerdeJsonFormatterAttribute";

    private const string flags_attr_fullname = $"{system_namespace}.{flags_attr_name}";
    private const string serde_enum_formatter_attr_fullname = $"{serde_json_namespace}.{serde_enum_formatter_attr_name}";
    private const string serde_json_serializable_attr_fullname = $"{serde_json_namespace}.{serde_json_serializable_attr_name}";
    private const string serde_json_member_attr_fullname = $"{serde_json_namespace}.{serde_json_member_attr_name}";
    private const string serde_json_ignore_member_attr_fullname = $"{serde_json_namespace}.{serde_json_ignore_member_attr_name}";
    private const string serde_json_formatter_attr_fullname = $"{serde_json_namespace}.{serde_json_formatter_attr_name}";

    internal const string nullable_type_fullname = $"{system_namespace}.{nuallable_type_name}";
    internal const string lazy_type_fullname = $"{system_namespace}.{lazy_type_name}";
    internal const string key_value_pair_type_fullname = $"{system_collection_generic_namespace}.{key_value_pair_type_name}";
    internal const string i_collection_type_fullname = $"{system_collection_generic_namespace}.{i_collection_type_name}";
    internal const string i_readonly_collection_type_fullname = $"{system_collection_generic_namespace}.{i_readonly_collection_type_name}";

    private readonly Compilation m_Compilation;

    // Attributes
    public INamedTypeSymbol flagsAttribute { get; }
    public INamedTypeSymbol serdeEnumFormatterAttribute { get; }
    public INamedTypeSymbol serdeJsonSerializableAttribute { get; }
    public INamedTypeSymbol serdeJsonMemberAttribute { get; }
    public INamedTypeSymbol serdeJsonIgnoreMemberAttribute { get; }
    public INamedTypeSymbol serdeJsonFormatterAttribute { get; }

    // Interfaces
    public INamedTypeSymbol iCollectionInterface { get; }
    public INamedTypeSymbol iReadonlyCollectionInterface { get; }
    public INamedTypeSymbol keyValuePairType { get; }
    public INamedTypeSymbol lazyType { get; }
    public INamedTypeSymbol nullableType { get; }

    public ReferenceSymbols(Compilation compilation)
    {
        m_Compilation = compilation;

        flagsAttribute = GetTypeByMetaDataName(flags_attr_fullname);
        serdeEnumFormatterAttribute = GetTypeByMetaDataName(serde_enum_formatter_attr_fullname);
        serdeJsonSerializableAttribute = GetTypeByMetaDataName(serde_json_serializable_attr_fullname);
        serdeJsonMemberAttribute = GetTypeByMetaDataName(serde_json_member_attr_fullname);
        serdeJsonIgnoreMemberAttribute = GetTypeByMetaDataName(serde_json_ignore_member_attr_fullname);
        serdeJsonFormatterAttribute = GetTypeByMetaDataName(serde_json_formatter_attr_fullname);
        iCollectionInterface = GetTypeByMetaDataName(i_collection_type_fullname);
        iReadonlyCollectionInterface = GetTypeByMetaDataName(i_readonly_collection_type_fullname);
        lazyType = GetTypeByMetaDataName(lazy_type_fullname);
        nullableType = GetTypeByMetaDataName(nullable_type_fullname);
        keyValuePairType = GetTypeByMetaDataName(key_value_pair_type_fullname);
    }

    public INamedTypeSymbol GetTypeByMetaDataName(string metaDataName)
        => m_Compilation.GetTypeByMetadataName(metaDataName) ?? throw new InvalidOperationException($"Type {metaDataName} is not found in compilation.");
}