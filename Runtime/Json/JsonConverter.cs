/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PeroTools2.Serde.Json;
using UnityEngine;

namespace PeroTools2.Serde
{
    /// <summary>
    /// Json 序列化器/反序列化器
    /// </summary>
    [PublicAPI]
    public static class JsonConverter
    {
        /// <summary>
        /// UTF-16 的 Json 序列化器/反序列化器
        /// </summary>
        public static class UTF16
        {
            /// <summary>
            /// 将对象序列化成 UTF-16 字符串
            /// </summary>
            /// <param name="value">要序列化的值</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>序列化或的 UTF-16 Json 字符串</returns>
            /// <exception cref="SerdeException">当该类型的格式化器从未注册时抛出</exception>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string Serialize<T>(T value)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    if (!SerdeJsonFormatterProvider.IsRegistered<T>())
                    {
                        if (BuiltinGenericFormatter<T>.formatter != null)
                        {
                            SerdeJsonFormatterProvider.Register(BuiltinGenericFormatter<T>.formatter);
                        }
                        else
                        {
                            throw SerdeException.ThrowFormatterNotFound(typeof(T));
                        }
                    }
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value);
                    jsonWriter.Flush();
                    return new string(bufferWriter.GetWrittenBuffer());
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// 将对象序列化成 UTF-16 字符串
            /// </summary>
            /// <param name="value">要序列化的值</param>
            /// <param name="formatter">格式化器</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>序列化或的 UTF-16 Json 字符串</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string Serialize<T>(T value, ISerdeJsonFormatter<T> formatter)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    return new string(bufferWriter.GetWrittenBuffer());
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// 将对象序列化成 UTF-16 字符串
            /// </summary>
            /// <param name="value">要序列化的值</param>
            /// <param name="formatter">格式化器</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>序列化或的 UTF-16 Json 字符串</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string Serialize<T>(T value, IFixedSerdeJsonFormatter<T> formatter)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    return new string(bufferWriter.GetWrittenBuffer());
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// 将 UTF-16 字符串反序列化成对象
            /// </summary>
            /// <param name="json">UTF-16 字符串</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>反序列化后的对象实例</returns>
            /// <exception cref="SerdeException">当该类型的格式化器从未注册时抛出</exception>
            public static T Deserialize<T>(ReadOnlySpan<char> json)
            {
                if (!SerdeJsonFormatterProvider.IsRegistered<T>())
                {
                    if (BuiltinGenericFormatter<T>.formatter != null)
                    {
                        SerdeJsonFormatterProvider.Register(BuiltinGenericFormatter<T>.formatter);
                    }
                    else
                    {
                        throw SerdeException.ThrowFormatterNotFound(typeof(T));
                    }
                }
                return new JsonReaderUtf16(json).ReadValue<T>();
            }

            /// <summary>
            /// 将 UTF-16 字符串反序列化成对象
            /// </summary>
            /// <param name="json">UTF-16 字符串</param>
            /// <param name="formatter">格式化器</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>反序列化后的对象实例</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Deserialize<T>(ReadOnlySpan<char> json, ISerdeJsonFormatter<T> formatter) => new JsonReaderUtf16(json).ReadValue(formatter);

            /// <summary>
            /// 将 UTF-16 字符串反序列化成对象
            /// </summary>
            /// <param name="json">UTF-16 字符串</param>
            /// <param name="formatter">格式化器</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>反序列化后的对象实例</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Deserialize<T>(ReadOnlySpan<char> json, IFixedSerdeJsonFormatter<T> formatter) => new JsonReaderUtf16(json).ReadValue(formatter);
        }

        public static class UTF8
        {
            /// <summary>
            /// 将对象序列化成 UTF-8 字符串
            /// </summary>
            /// <param name="value">要序列化的值</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>序列化或的 UTF-8 Json 字符串</returns>
            /// <exception cref="SerdeException">当该类型的格式化器从未注册时抛出</exception>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static byte[] Serialize<T>(T value)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    if (!SerdeJsonFormatterProvider.IsRegistered<T>())
                    {
                        if (BuiltinGenericFormatter<T>.formatter != null)
                        {
                            SerdeJsonFormatterProvider.Register(BuiltinGenericFormatter<T>.formatter);
                        }
                        else
                        {
                            throw SerdeException.ThrowFormatterNotFound(typeof(T));
                        }
                    }
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value);
                    jsonWriter.Flush();
                    return bufferWriter.CopyToArray();
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// 将对象序列化成 UTF-8 字符串
            /// </summary>
            /// <param name="value">要序列化的值</param>
            /// <param name="formatter">格式化器</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>序列化或的 UTF-8 Json 字符串</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static byte[] Serialize<T>(T value, ISerdeJsonFormatter<T> formatter)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    return bufferWriter.CopyToArray();
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// 将对象序列化成 UTF-8 字符串
            /// </summary>
            /// <param name="value">要序列化的值</param>
            /// <param name="formatter">格式化器</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>序列化或的 UTF-8 Json 字符串</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static byte[] Serialize<T>(T value, IFixedSerdeJsonFormatter<T> formatter)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    return bufferWriter.CopyToArray();
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// 将 UTF-8 字符串反序列化成对象
            /// </summary>
            /// <param name="json">UTF-8 字符串</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>反序列化后的对象实例</returns>
            /// <exception cref="SerdeException">当该类型的格式化器从未注册时抛出</exception>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Deserialize<T>(ReadOnlySpan<byte> json)
            {
                if (!SerdeJsonFormatterProvider.IsRegistered<T>())
                {
                    if (BuiltinGenericFormatter<T>.formatter != null)
                    {
                        SerdeJsonFormatterProvider.Register(BuiltinGenericFormatter<T>.formatter);
                    }
                    else
                    {
                        throw SerdeException.ThrowFormatterNotFound(typeof(T));
                    }
                }
                return new JsonReaderUtf8(json).ReadValue<T>();
            }

            /// <summary>
            /// 将 UTF-8 字符串反序列化成对象
            /// </summary>
            /// <param name="json">UTF-8 字符串</param>
            /// <param name="formatter">格式化器</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>反序列化后的对象实例</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Deserialize<T>(ReadOnlySpan<byte> json, ISerdeJsonFormatter<T> formatter) => new JsonReaderUtf8(json).ReadValue(formatter);

            /// <summary>
            /// 将 UTF-8 字符串反序列化成对象
            /// </summary>
            /// <param name="json">UTF-8 字符串</param>
            /// <param name="formatter">格式化器</param>
            /// <typeparam name="T">序列化值的类型</typeparam>
            /// <returns>反序列化后的对象实例</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Deserialize<T>(ReadOnlySpan<byte> json, IFixedSerdeJsonFormatter<T> formatter) => new JsonReaderUtf8(json).ReadValue(formatter);
        }

        /// <summary>
        /// 内置类型的泛型格式化器，避免直接调用内置泛型类型的序列化和反序列化时导致 Formatter 找不到
        /// </summary>
        // TODO: 后续需要根据调用 Serialize/Deserialize 来生成默认的 BuiltinFormatter 并注册，以免 IL2CPP 裁剪或找不到泛型实现
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static class BuiltinGenericFormatter<T>
        {
            internal static readonly ISerdeJsonFormatter<T> formatter;

            static BuiltinGenericFormatter()
            {
                if (typeof(T).IsArray)
                {
                    var type = typeof(ArrayFormatter<>).MakeGenericType(typeof(T).GetElementType());
                    formatter = Activator.CreateInstance(type) as ISerdeJsonFormatter<T>;
                    return;
                }
                if (typeof(T).IsEnum)
                {
                    var type = typeof(EnumNumberFormatter<>).MakeGenericType(typeof(T).GetElementType());
                    formatter = Activator.CreateInstance(type) as ISerdeJsonFormatter<T>;
                    return;
                }
                if (IsCollection(typeof(T)))
                {
                    var type = typeof(ArrayFormatter<>).MakeGenericType(typeof(T).GenericTypeArguments[0]);
                    formatter = Activator.CreateInstance(type) as ISerdeJsonFormatter<T>;
                    return;
                }
                if (IsKeyValuePair(typeof(T)))
                {
                    var args = typeof(T).GenericTypeArguments;
                    var type = typeof(KeyValuePairFormatter<,>).MakeGenericType(args[0], args[1]);
                    formatter = Activator.CreateInstance(type) as ISerdeJsonFormatter<T>;
                    return;
                }
                if (IsNullable(typeof(T)))
                {
                    var type = typeof(NullableFormatter<>).MakeGenericType(typeof(T).GenericTypeArguments[0]);
                    formatter = Activator.CreateInstance(type) as ISerdeJsonFormatter<T>;
                    return;
                }
                if (IsLazy(typeof(T)))
                {
                    var type = typeof(LazyFormatter<>).MakeGenericType(typeof(T).GenericTypeArguments[0]);
                    formatter = Activator.CreateInstance(type) as ISerdeJsonFormatter<T>;
                }
            }

            private static bool IsCollection(Type type)
            {
                foreach (var iType in type.GetInterfaces())
                {
                    if (iType.Name.Equals("ICollection`1") && iType.Namespace.Equals("System.Collections.Generic") ||
                        iType.Name.Equals("IReadOnlyCollection`1") && iType.Namespace.Equals("System.Collections.Generic"))
                        return true;
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsNullable(Type type) =>
                type.Name.Equals("Nullable`1") && type.Namespace.Equals("System");

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsLazy(Type type) =>
                type.Name.Equals("Lazy`1") && type.Namespace.Equals("System");

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsKeyValuePair(Type type) =>
                type.Name.Equals("KeyValuePair`2") && type.Namespace.Equals("System.Collections.Generic");
        }
    }
}