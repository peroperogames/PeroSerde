/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/07
 * Description: SerdeJsonFormatterProvider
 */

using System.Runtime.CompilerServices;
using System.Threading;

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

    /// <summary>
    /// Whether the hot-path cache already resolved a formatter for <typeparamref name="T"/>. Callers use it to skip the
    /// generic static lookups (and the generic cache's type initializer check) on every deserialize call.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsResolved<T>() =>
        Dispatch<T>.version == version && (Dispatch<T>.fixedFormatter != null || Dispatch<T>.unfixedFormatter != null);

    /// <summary>Bumped on every registration so the per-call caches know they have to resolve again</summary>
    internal static volatile int version;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Register<T>(ISerdeJsonFormatter<T> formatter)
    {
        Cache<T>.registered = true;
        Cache<T>.formatter = formatter;
        version++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Register<T>(IFixedSerdeJsonFormatter<T> formatter)
    {
        FixedCache<T>.registered = true;
        FixedCache<T>.formatter = formatter;
        version++;
    }

    /// <summary>
    /// Resolves the formatter of <typeparamref name="T"/> once per registration, for the hot paths that call a
    /// formatter per member. Reading <see cref="version"/> and comparing it is cheaper than the three generic
    /// static reads plus the interface casts that <see cref="GetFormatterInternal{T}"/> performs on every call.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Resolve<T>(out IFixedSerdeJsonFormatter<T> fixedFormatter, out ISerdeJsonFormatter<T> unfixedFormatter)
    {
        if (Dispatch<T>.version != version)
        {
            ResolveSlow<T>();
        }
        fixedFormatter = Dispatch<T>.fixedFormatter;
        unfixedFormatter = Dispatch<T>.unfixedFormatter;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ResolveSlow<T>()
    {
        var formatter = GetFormatterInternal<T>();
        Dispatch<T>.fixedFormatter = formatter as IFixedSerdeJsonFormatter<T>;
        Dispatch<T>.unfixedFormatter = formatter as ISerdeJsonFormatter<T>;
        // Publish the formatter fields before the version that vouches for them, so another thread that reads the new
        // version can never observe a half written cache
        Volatile.Write(ref Dispatch<T>.version, Volatile.Read(ref version));
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

    private static class Dispatch<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public static int version = -1;
        public static IFixedSerdeJsonFormatter<T> fixedFormatter;
        public static ISerdeJsonFormatter<T> unfixedFormatter;
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