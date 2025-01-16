/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/07
 * Description: SerdeJsonFormatterProvider
 */

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
            RegisterBuiltinFormatters();
        }

        private static void RegisterBuiltinFormatters()
        {
            Register(new StringFormatter());
            Register(new GuidFormatter());
            Register(new Vector2Formatter());
            Register(new Vector3Formatter());
            Register(new Vector4Formatter());
            Register(new ColorFormatter());
            Register(new DateTimeFormatter());
        }

        public static bool IsRegistered<T>() => Cache<T>.registered || FixedCache<T>.registered;

        public static void Register<T>(ISerdeJsonFormatter<T> formatter)
        {
            Cache<T>.registered = true;
            Cache<T>.formatter = formatter;
        }

        public static void Register<T>(IFixedSerdeJsonFormatter<T> formatter)
        {
            FixedCache<T>.registered = true;
            FixedCache<T>.formatter = formatter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetFormatterInternal<T>()
        {
            if (Cache<T>.registered)
            {
                // 类型已注册, 直接返回
                return Cache<T>.formatter;
            }
            if (FixedCache<T>.registered)
            {
                // 类型已注册, 直接返回
                return FixedCache<T>.formatter;
            }
            // il2cpp下不应该使用MakeGeneric生成泛型Formatter. 应该改为初始化时主动注册.
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerdeJsonFormatter<T> GetFormatter<T>()
        {
            if (Cache<T>.registered)
            {
                // 类型已注册, 直接返回
                return Cache<T>.formatter;
            }

            // il2cpp下不应该使用MakeGeneric生成泛型Formatter. 应该改为初始化时主动注册.
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IFixedSerdeJsonFormatter<T> GetFixedFormatter<T>()
        {
            if (FixedCache<T>.registered)
            {
                // 类型已注册, 直接返回
                return FixedCache<T>.formatter;
            }

            // il2cpp下不应该使用MakeGeneric生成泛型Formatter. 应该改为初始化时主动注册.
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }

        private static class Cache<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            public static bool registered;
            public static ISerdeJsonFormatter<T> formatter;
        }

        private static class FixedCache<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            public static bool registered;
            public static IFixedSerdeJsonFormatter<T> formatter;
        }
    }
}