/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/07
 * Description: SerdeJsonFormatterProvider
 */

using System.Reflection;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// Json 格式化器的全局缓存
    /// </summary>
    public static partial class SerdeJsonFormatterProvider
    {
        static SerdeJsonFormatterProvider()
        {
            RegisterPrimitiveFormatters();
            RegisterBuildInFormatters();
        }

        private static void RegisterBuildInFormatters()
        {
            Register(new StringFormatter());
            Register(new GuidFormatter());
            Register(new Vector2Formatter());
            Register(new Vector3Formatter());
            Register(new Vector4Formatter());
            Register(new ColorFormatter());
            Register(new DateTimeFormatter());
        }

        private static class Cache<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            public static bool registered;
            public static ISerdeJsonFormatter<T> formatter;
        }

        public static bool IsRegistered<T>() => Cache<T>.registered;

        public static void Register<T>(ISerdeJsonFormatter<T> formatter)
        {
            Cache<T>.registered = true;
            Cache<T>.formatter = formatter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerdeJsonFormatter<T> GetFormatter<T>()
        {
            if (Cache<T>.registered || TryRegisterFormatter<T>())
            {
                // 类型已注册, 直接返回
                return Cache<T>.formatter;
            }

            // il2cpp下不应该使用MakeGeneric生成泛型Formatter. 应该改为初始化时主动注册.
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryRegisterFormatter<T>()
        {
            var method = typeof(T).GetMethod($"RegisterFormattersFor{typeof(T).Name}", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null) return false;
            method.Invoke(null, null);
            return Cache<T>.registered;
        }
    }
}