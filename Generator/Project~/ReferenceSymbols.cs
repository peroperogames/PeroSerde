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

    private const string dictionary_type_name = "Dictionary`2";
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
    internal const string dictionary_type_fullname = $"{system_collection_generic_namespace}.{dictionary_type_name}";

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
    public INamedTypeSymbol dictionaryType { get; }

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
        dictionaryType = GetTypeByMetaDataName(dictionary_type_fullname);
    }

    public INamedTypeSymbol GetTypeByMetaDataName(string metaDataName)
        => Resolve(metaDataName) ?? throw new InvalidOperationException($"Type {metaDataName} is not found in compilation.");

    /// <summary>
    /// 解析框架类型符号。
    /// <para>
    /// 引用集中可能存在多份同名定义，此时 <see cref="Compilation.GetTypeByMetadataName(string)"/>
    /// 会因歧义返回 null（该方法不考虑可见性）。例如 Unity 2021.3 下 NUnit 3.5 的 net35 版本自带一份
    /// internal 的 System.Lazy`1，与 mscorlib 冲突，会导致整个生成器中断。
    /// 这里逐个程序集回退查找，并优先选择公开定义。
    /// </para>
    /// </summary>
    private INamedTypeSymbol? Resolve(string metaDataName)
    {
        var resolved = m_Compilation.GetTypeByMetadataName(metaDataName);
        if (resolved != null)
        {
            return resolved;
        }

        INamedTypeSymbol? nonPublicCandidate = null;
        foreach (var reference in m_Compilation.References)
        {
            if (m_Compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assembly)
            {
                continue;
            }

            resolved = assembly.GetTypeByMetadataName(metaDataName);
            if (resolved == null)
            {
                continue;
            }

            if (resolved.DeclaredAccessibility == Accessibility.Public)
            {
                return resolved;
            }

            nonPublicCandidate ??= resolved;
        }

        return nonPublicCandidate;
    }
}