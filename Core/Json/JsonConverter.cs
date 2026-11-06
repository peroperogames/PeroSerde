/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// JSON serializer/deserializer
    /// </summary>
    public static class JsonConverter
    {
        /// <summary>
        /// UTF-16 JSON serializer/deserializer
        /// </summary>
        public static class UTF16
        {
            static UTF16()
            {
                ReusableBufferWriter<char>.cachePool = ArrayPool.UTF16;
            }

            /// <summary>
            /// Serializes an object into a UTF-16 string
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The serialized UTF-16 JSON string</returns>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
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
            /// Serializes an object into a UTF-16 string
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The serialized UTF-16 JSON string</returns>
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
            /// Serializes an object into a UTF-16 string
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The serialized UTF-16 JSON string</returns>
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
            /// Deserializes a UTF-16 string into an object
            /// </summary>
            /// <param name="json">The UTF-16 string</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The deserialized object instance</returns>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            public static unsafe T Deserialize<T>(ReadOnlySpan<char> json)
            {
                if (TrimWhiteSpace(json, out var jsonTrimmedWhiteSpace))
                {
                    return default;
                }

                jsonTrimmedWhiteSpace = JsonBufferSentinel.PadUtf16(jsonTrimmedWhiteSpace);

                if (!SerdeJsonFormatterProvider.IsResolved<T>() && !SerdeJsonFormatterProvider.IsRegistered<T>())
                {
                    if (BuiltinGenericFormatter<T>.formatter != null)
                    {
                        SerdeJsonFormatterProvider.Register(BuiltinGenericFormatter<T>.formatter);
                    }
                    else
                    {
                        // GetFormatter resolves the source-generated formatter as a last resort.
                        SerdeJsonFormatterProvider.GetFormatter<T>();
                    }
                }
                try
                {
                    return new JsonReaderUtf16(jsonTrimmedWhiteSpace).ReadValue<T>();
                }
                catch (IndexOutOfRangeException)
                {
                    // The fast paths read past the last token, so a truncated document can run past the buffer; report
                    // it as invalid json instead of leaking the index error
                    throw SerdeException.ThrowInvalidJson(jsonTrimmedWhiteSpace.ToString());
                }
            }

            /// <summary>
            /// Deserializes a UTF-16 string into an object
            /// </summary>
            /// <param name="json">The UTF-16 string</param>
            /// <param name="formatter">The formatter</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The deserialized object instance</returns>
            public static unsafe T Deserialize<T>(ReadOnlySpan<char> json, ISerdeJsonFormatter<T> formatter)
            {
                if (TrimWhiteSpace(json, out var jsonTrimmedWhiteSpace))
                {
                    return default;
                }

                jsonTrimmedWhiteSpace = JsonBufferSentinel.PadUtf16(jsonTrimmedWhiteSpace);
                return new JsonReaderUtf16(jsonTrimmedWhiteSpace).ReadValue(formatter);
            }

            /// <summary>
            /// Deserializes a UTF-16 string into an object
            /// </summary>
            /// <param name="json">The UTF-16 string</param>
            /// <param name="formatter">The formatter</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The deserialized object instance</returns>
            public static unsafe T Deserialize<T>(ReadOnlySpan<char> json, IFixedSerdeJsonFormatter<T> formatter)
            {
                if (TrimWhiteSpace(json, out var jsonTrimmedWhiteSpace))
                {
                    return default;
                }

                jsonTrimmedWhiteSpace = JsonBufferSentinel.PadUtf16(jsonTrimmedWhiteSpace);
                return new JsonReaderUtf16(jsonTrimmedWhiteSpace).ReadValue(formatter);
            }

            /// <summary>
            /// Trims the leading whitespace characters
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="trimmed"></param>
            /// <returns>True if everything is whitespace, otherwise false</returns>
            private static bool TrimWhiteSpace(ReadOnlySpan<char> origin, out ReadOnlySpan<char> trimmed)
            {
                if (origin.IsEmpty)
                {
                    trimmed = origin;
                    return true;
                }

                for (var i = 0; i < origin.Length; i++)
                {
                    if (origin[i] != ' ')
                    {
                        trimmed = origin.Slice(i, origin.Length - i);
                        return false;
                    }
                }
                trimmed = origin;
                return true;
            }
        }

        public static class UTF8
        {
            static UTF8()
            {
                ReusableBufferWriter<byte>.cachePool = ArrayPool.UTF8;
            }

            /// <summary>
            /// Serializes an object into a UTF-8 string
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The serialized UTF-8 JSON string</returns>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
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
            /// Serializes an object into a UTF-8 string
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The serialized UTF-8 JSON string</returns>
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
            /// Serializes an object into a UTF-8 string
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The serialized UTF-8 JSON string</returns>
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
            /// Deserializes a UTF-8 string into an object
            /// </summary>
            /// <param name="json">The UTF-8 string</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The deserialized object instance</returns>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            public static unsafe T Deserialize<T>(ReadOnlySpan<byte> json)
            {
                if (TrimWhiteSpace(json, out var jsonTrimmedWhiteSpace))
                {
                    return default;
                }

                jsonTrimmedWhiteSpace = JsonBufferSentinel.PadUtf8(jsonTrimmedWhiteSpace);

                if (!SerdeJsonFormatterProvider.IsResolved<T>() && !SerdeJsonFormatterProvider.IsRegistered<T>())
                {
                    if (BuiltinGenericFormatter<T>.formatter != null)
                    {
                        SerdeJsonFormatterProvider.Register(BuiltinGenericFormatter<T>.formatter);
                    }
                    else
                    {
                        // GetFormatter resolves the source-generated formatter as a last resort.
                        SerdeJsonFormatterProvider.GetFormatter<T>();
                    }
                }
                try
                {
                    return new JsonReaderUtf8(jsonTrimmedWhiteSpace).ReadValue<T>();
                }
                catch (IndexOutOfRangeException)
                {
                    throw SerdeException.ThrowInvalidJson(System.Text.Encoding.UTF8.GetString(jsonTrimmedWhiteSpace));
                }
            }

            /// <summary>
            /// Deserializes a UTF-8 string into an object
            /// </summary>
            /// <param name="json">The UTF-8 string</param>
            /// <param name="formatter">The formatter</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The deserialized object instance</returns>
            public static unsafe T Deserialize<T>(ReadOnlySpan<byte> json, ISerdeJsonFormatter<T> formatter)
            {
                if (TrimWhiteSpace(json, out var jsonTrimmedWhiteSpace))
                {
                    return default;
                }

                jsonTrimmedWhiteSpace = JsonBufferSentinel.PadUtf8(jsonTrimmedWhiteSpace);
                return new JsonReaderUtf8(jsonTrimmedWhiteSpace).ReadValue(formatter);
            }

            /// <summary>
            /// Deserializes a UTF-8 string into an object
            /// </summary>
            /// <param name="json">The UTF-8 string</param>
            /// <param name="formatter">The formatter</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The deserialized object instance</returns>
            public static unsafe T Deserialize<T>(ReadOnlySpan<byte> json, IFixedSerdeJsonFormatter<T> formatter)
            {
                if (TrimWhiteSpace(json, out var jsonTrimmedWhiteSpace))
                {
                    return default;
                }

                jsonTrimmedWhiteSpace = JsonBufferSentinel.PadUtf8(jsonTrimmedWhiteSpace);
                return new JsonReaderUtf8(jsonTrimmedWhiteSpace).ReadValue(formatter);
            }

            /// <summary>
            /// Trims the leading whitespace characters
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="trimmed"></param>
            /// <returns>True if everything is whitespace, otherwise false</returns>
            private static bool TrimWhiteSpace(ReadOnlySpan<byte> origin, out ReadOnlySpan<byte> trimmed)
            {
                if (origin.IsEmpty)
                {
                    trimmed = origin;
                    return true;
                }

                for (var i = 0; i < origin.Length; i++)
                {
                    if (origin[i] != (byte) ' ')
                    {
                        trimmed = origin.Slice(i, origin.Length - i);
                        return false;
                    }
                }
                trimmed = origin;
                return true;
            }
        }

        /// <summary>
        /// Generic formatter for built-in types, so that serializing and deserializing a built-in generic type directly does not fail to find a Formatter
        /// </summary>
        // TODO: generate and register a default BuiltinFormatter on Serialize/Deserialize calls later, to avoid IL2CPP stripping or a missing generic implementation
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
                    var type = typeof(CollectionFormatter<,>).MakeGenericType(typeof(T), typeof(T).GenericTypeArguments[0]);
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

/// <summary>
/// Appends a trailing space when the trimmed buffer does not end with a JSON delimiter or whitespace.
/// The primitive parsers rely on a terminator byte after the value, so a root-level scalar that sits
/// exactly at the end of the buffer would otherwise read past the buffer.
/// </summary>
internal static class JsonBufferSentinel
{
    internal static bool NeedsPadding(byte last) =>
        last != (byte) '}' && last != (byte) ']' && last != (byte) ',' && last != (byte) '"'
        && !PeroTools2.Serde.StringHelper.IsWhiteSpace(last);

    internal static bool NeedsPadding(char last) =>
        last != '}' && last != ']' && last != ',' && last != '"'
        && !PeroTools2.Serde.StringHelper.IsWhiteSpace(last);

    internal static ReadOnlySpan<byte> PadUtf8(ReadOnlySpan<byte> json)
    {
        if (json.Length == 0 || !NeedsPadding(json[^1]))
        {
            return json;
        }

        var padded = new byte[json.Length + 1];
        json.CopyTo(padded);
        padded[json.Length] = (byte) ' ';
        return padded;
    }

    internal static ReadOnlySpan<char> PadUtf16(ReadOnlySpan<char> json)
    {
        if (json.Length == 0 || !NeedsPadding(json[^1]))
        {
            return json;
        }

        var padded = new char[json.Length + 1];
        json.CopyTo(padded);
        padded[json.Length] = ' ';
        return padded;
    }
}