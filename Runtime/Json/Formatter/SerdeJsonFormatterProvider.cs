/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/07
 * Description: SerdeJsonFormatterProvider
 */

using System;
using System.Reflection;

namespace PeroTools2.Serde.Json.Formatter
{
    public static partial class SerdeJsonFormatterProvider
    {
        static SerdeJsonFormatterProvider()
        {
            RegisterPrimitiveFormatters();
            RegisterBuildInFormatters();
        }

        static partial void RegisterPrimitiveFormatters();
        static partial void RegisterBuildInFormatters();

        // TODO Non-Generic方法支持?
        // TODO 初始化时自动注册生成的CustomFormatter(使用UnityInitializeOnLoad)
        private static class Cache<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            public static bool registered;
            public static SerdeJsonFormatter<T> formatter;
        }

        public static bool IsRegistered<T>() => Cache<T>.registered;

        public static void Register<T>(SerdeJsonFormatter<T> formatter)
        {
            // TODO Cache<T>能不能在IL2CPP中自动Preserve?
            Cache<T>.registered = true;
            Cache<T>.formatter = formatter;
        }

        public static SerdeJsonFormatter<T> GetFormatter<T>()
        {
            if (Cache<T>.registered)
            {
                // 类型已注册, 直接返回
                return Cache<T>.formatter;
            }
            // 查找类型是否有静态注册函数, 如果有就手动调用一次. 静态构造函数会负责注册
            if (TryInvokeRegisterFormatter(typeof(T)) && Cache<T>.registered)
            {
                return Cache<T>.formatter;
            }
            // il2cpp下不应该使用MakeGeneric生成泛型Formatter. 应该改为初始化时主动注册.
            SerdeException.ThrowFormatterNotFound(typeof(T));
            return default;
        }

        private static bool TryInvokeRegisterFormatter(Type type)
        {
            if (!typeof(ISerdeJsonSerializable).IsAssignableFrom(type))
            {
                return false;
            }
            var registerMethod = type.GetMethod("RegisterJsonFormatter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (registerMethod == null)
            {
                throw new InvalidOperationException($"Type \"{type.FullName}\" implements {nameof(ISerdeJsonSerializable)} but can not found RegisterJsonFormatter method.");
            }
            registerMethod.Invoke(null, null);
            return true;
        }
    }
}