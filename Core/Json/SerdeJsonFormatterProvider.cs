/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/07
 * Description: SerdeJsonFormatterProvider
 */

using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

/// <summary>
/// Global cache of JSON formatters
/// </summary>
public static partial class SerdeJsonFormatterProvider
{
    static SerdeJsonFormatterProvider()
    {
        RegisterPrimitiveFormatters();
        RegisterBuiltinFormatters();
    }

    internal static void RegisterBuiltinFormatters()
    {
        Register(new StringFormatter());
        Register(new GuidFormatter());
        Register(new DateTimeFormatter());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRegistered<T>() => Cache<T>.registered || FixedCache<T>.registered || GenericCache<T>.registered;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Register<T>(ISerdeJsonFormatter<T> formatter)
    {
        Cache<T>.registered = true;
        Cache<T>.formatter = formatter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            // Type already registered; return it directly
            return Cache<T>.formatter;
        }
        if (FixedCache<T>.registered)
        {
            // Type already registered; return it directly
            return FixedCache<T>.formatter;
        }
        // GenericCache runs the generated registry from its static constructor, which is what Unity
        // does eagerly through RuntimeInitializeOnLoadMethod; other hosts pay it lazily here.
        if (GenericCache<T>.registered)
        {
            return Cache<T>.formatter;
        }
        // MakeGeneric must not be used to create generic Formatters under il2cpp. Register them explicitly during initialization instead.
        throw SerdeException.ThrowFormatterNotFound(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ISerdeJsonFormatter<T> GetFormatter<T>()
    {
        if (Cache<T>.registered)
        {
            // Type already registered; return it directly
            return Cache<T>.formatter;
        }

        // MakeGeneric must not be used to create generic Formatters under il2cpp. Register them explicitly during initialization instead.
        throw SerdeException.ThrowFormatterNotFound(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFixedSerdeJsonFormatter<T> GetFixedFormatter<T>()
    {
        if (FixedCache<T>.registered)
        {
            // Type already registered; return it directly
            return FixedCache<T>.formatter;
        }

        // MakeGeneric must not be used to create generic Formatters under il2cpp. Register them explicitly during initialization instead.
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

    private static class GenericCache<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public static bool registered => Cache<T>.registered;

        /// <summary>
        /// Generic types cannot be registered eagerly; the static constructor must be called explicitly
        /// </summary>
        static GenericCache() => RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
    }
}