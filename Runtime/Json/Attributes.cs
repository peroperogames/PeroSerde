/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/28
 */

using System;

namespace PeroTools2.Serde.Json
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Struct |
        AttributeTargets.Enum |
        AttributeTargets.Interface,
        Inherited = false
    )]
    public sealed class SerdeJsonSerializableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerdeJsonMemberAttribute : Attribute
    {
        public SerdeJsonMemberAttribute(string formatName) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerdeJsonIgnoreMemberAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SerdeJsonFormatterAttribute : Attribute
    {
        public SerdeJsonFormatterAttribute(Type formatterType) { }
        public SerdeJsonFormatterAttribute(Type keyFormatterType, Type valueForammterType) { }
    }
}