/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/28
 * Description: Attributes
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
    public sealed class SerdeJsonSerializableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerdeJsonMemberAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerdeJsonIncludeMemberAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerdeJsonIgnoreMemberAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class SerdeJsonConstructorAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Struct)]
    public sealed class SerdeJsonCustomFormatterAttribute : Attribute
    {
        public Type ofType { get; }

        public SerdeJsonCustomFormatterAttribute(Type ofType)
        {
            this.ofType = ofType;
        }
    }
}