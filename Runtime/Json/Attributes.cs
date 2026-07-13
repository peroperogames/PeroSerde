/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23-17:41:00
 */

using System;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 为该类型自动添加 Formatter
    /// </summary>
    [PublicAPI]
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Struct |
        AttributeTargets.Enum,
        Inherited = false
    )]
    public sealed class SerdeJsonSerializableAttribute : Attribute
    {
        /// <param name="ignoreMissingMembers">若为 true，则在反序列化时忽略缺失的成员</param>
        /// <param name="ignoreNullMembers">若为 true，则在序列化时忽略值为 null 的成员</param>
        public SerdeJsonSerializableAttribute(bool ignoreMissingMembers = true, bool ignoreNullMembers = false) { }
    }

    /// <summary>
    /// 为 Serde 指示在 JSON 序列化和反序列化时使用哪个名称
    /// </summary>
    [PublicAPI, AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerdeJsonMemberAttribute : Attribute
    {
        /// <summary>
        /// 为 Serde 指示在 JSON 序列化和反序列化时使用哪个名称
        /// </summary>
        /// <param name="formatName">使用的名称</param>
        public SerdeJsonMemberAttribute(string formatName) { }
    }

    /// <summary>
    /// 向 Serde 指示在 JSON 序列化和反序列化时忽略该属性或字段
    /// </summary>
    [PublicAPI, AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerdeJsonIgnoreMemberAttribute : Attribute { }

    /// <summary>
    /// 向 Serde 指示在 JSON 序列化和反序列化时使用哪个 JSON 格式器
    /// </summary>
    [PublicAPI, AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SerdeJsonFormatterAttribute : Attribute
    {
        /// <summary>
        /// 向 Serde 指示在 JSON 序列化和反序列化时使用哪个 JSON 格式器
        /// </summary>
        /// <param name="formatterType">格式化器类型</param>
        public SerdeJsonFormatterAttribute(Type formatterType) { }
    }

    /// <summary>
    /// 向 Serde 指示使用何种方式为枚举类型提供默认的序列化器和反序列化器
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Enum)]
    public class SerdeJsonEnumFormatterAttribute : Attribute
    {
        /// <summary>
        /// 向 Serde 指示使用何种方式为枚举类型提供默认的序列化器和反序列化器
        /// </summary>
        /// <param name="serializationFormat">序列化时使用的格式化类型</param>
        /// <param name="deserializationFormat">反序列化时使用的格式化类型</param>
        public SerdeJsonEnumFormatterAttribute(SerdeEnumFormat serializationFormat = SerdeEnumFormat.Number, SerdeEnumFormat deserializationFormat = SerdeEnumFormat.Number) { }
    }

    [PublicAPI]
    public enum SerdeEnumFormat
    {
        /// <summary>
        /// 向 Serde 指示使用字符串的方式为枚举提供序列化器或反序列化器
        /// </summary>
        String = 0,

        /// <summary>
        /// 向 Serde 指示使用数字类型的方式为枚举提供序列化器或反序列化器
        /// </summary>
        Number = 1
    }
}