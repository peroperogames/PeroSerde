/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
            static UTF16() => ReusableBufferWriter<char>.cachePool = ArrayPool.UTF16;

            #region Serialize Standard

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
                    return new string(bufferWriter.GetWrittenSpan());
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
                    return new string(bufferWriter.GetWrittenSpan());
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
                    return new string(bufferWriter.GetWrittenSpan());
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            #endregion

            #region Serialize TextWriter

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="TextWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="writer">The writer to write the serialized JSON to; must not be null</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single write call, so
            /// <paramref name="writer"/> never observes a partially written document
            /// </remarks>
            public static void Serialize<T>(T value, TextWriter writer)
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
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="TextWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The writer to write the serialized JSON to; must not be null</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single write call, so
            /// <paramref name="writer"/> never observes a partially written document
            /// </remarks>
            public static void Serialize<T>(T value, ISerdeJsonFormatter<T> formatter, TextWriter writer)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="TextWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The writer to write the serialized JSON to; must not be null</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single write call, so
            /// <paramref name="writer"/> never observes a partially written document
            /// </remarks>
            public static void Serialize<T>(T value, IFixedSerdeJsonFormatter<T> formatter, TextWriter writer)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Asynchronously serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="TextWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="writer">The writer to write the serialized JSON to</param>
            /// <param name="cancellationToken">The token to cancel the asynchronous write operation</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>A task that represents the asynchronous write operation</returns>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single asynchronous
            /// write call. Cancelling the token or failing the write can leave a partially written document in the target,
            /// nothing is rolled back, so the caller has to discard or rebuild it
            /// </remarks>
            public static async ValueTask SerializeAsync<T>(T value, TextWriter writer, CancellationToken cancellationToken = default)
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
                    var buffer = bufferWriter.GetWrittenMemory();
                    await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Asynchronously serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="TextWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The writer to write the serialized JSON to</param>
            /// <param name="cancellationToken">The token to cancel the asynchronous write operation</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>A task that represents the asynchronous write operation</returns>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single asynchronous
            /// write call. Cancelling the token or failing the write can leave a partially written document in the target,
            /// nothing is rolled back, so the caller has to discard or rebuild it
            /// </remarks>
            public static async ValueTask SerializeAsync<T>(T value, ISerdeJsonFormatter<T> formatter, TextWriter writer, CancellationToken cancellationToken = default)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenMemory();
                    await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Asynchronously serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="TextWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The writer to write the serialized JSON to</param>
            /// <param name="cancellationToken">The token to cancel the asynchronous write operation</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>A task that represents the asynchronous write operation</returns>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single asynchronous
            /// write call. Cancelling the token or failing the write can leave a partially written document in the target,
            /// nothing is rolled back, so the caller has to discard or rebuild it
            /// </remarks>
            public static async ValueTask SerializeAsync<T>(T value, IFixedSerdeJsonFormatter<T> formatter, TextWriter writer, CancellationToken cancellationToken = default)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenMemory();
                    await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            #endregion

            #region Serialize BufferWriter

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="IBufferWriter{T}"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="writer">The buffer writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            public static void Serialize<T>(T value, IBufferWriter<char> writer)
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
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="IBufferWriter{T}"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The buffer writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            public static void Serialize<T>(T value, ISerdeJsonFormatter<T> formatter, IBufferWriter<char> writer)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="IBufferWriter{T}"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The buffer writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            public static void Serialize<T>(T value, IFixedSerdeJsonFormatter<T> formatter, IBufferWriter<char> writer)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            #endregion

            #region Serialize BinaryWriter

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="BinaryWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="writer">The binary writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            /// <remarks>
            /// <c>BinaryWriter.Write(ReadOnlySpan&lt;char&gt;)</c> encodes the characters with <see cref="BinaryWriter.Encoding"/>
            /// and writes no byte order mark, so a default <see cref="BinaryWriter"/> stores UTF-8 bytes even though this
            /// overload belongs to the <see cref="UTF16"/> entry. Construct the writer with <c>Encoding.Unicode</c> when raw
            /// UTF-16 bytes are required, and prefer <see cref="TextWriter"/> when the target works with characters
            /// </remarks>
            public static void Serialize<T>(T value, BinaryWriter writer)
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
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="BinaryWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The binary writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <remarks>
            /// <c>BinaryWriter.Write(ReadOnlySpan&lt;char&gt;)</c> encodes the characters with <see cref="BinaryWriter.Encoding"/>
            /// and writes no byte order mark, so a default <see cref="BinaryWriter"/> stores UTF-8 bytes even though this
            /// overload belongs to the <see cref="UTF16"/> entry. Construct the writer with <c>Encoding.Unicode</c> when raw
            /// UTF-16 bytes are required, and prefer <see cref="TextWriter"/> when the target works with characters
            /// </remarks>
            public static void Serialize<T>(T value, ISerdeJsonFormatter<T> formatter, BinaryWriter writer)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into a UTF-16 JSON string and writes it to the specified <see cref="BinaryWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The binary writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <remarks>
            /// <c>BinaryWriter.Write(ReadOnlySpan&lt;char&gt;)</c> encodes the characters with <see cref="BinaryWriter.Encoding"/>
            /// and writes no byte order mark, so a default <see cref="BinaryWriter"/> stores UTF-8 bytes even though this
            /// overload belongs to the <see cref="UTF16"/> entry. Construct the writer with <c>Encoding.Unicode</c> when raw
            /// UTF-16 bytes are required, and prefer <see cref="TextWriter"/> when the target works with characters
            /// </remarks>
            public static void Serialize<T>(T value, IFixedSerdeJsonFormatter<T> formatter, BinaryWriter writer)
            {
                var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf16(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<char>.Pool.Return(bufferWriter);
                }
            }

            #endregion

            #region Deserialize Standard

            /// <summary>
            /// Deserializes a UTF-16 string into an object
            /// </summary>
            /// <param name="json">The UTF-16 string</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The deserialized object instance</returns>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            public static T Deserialize<T>(ReadOnlySpan<char> json)
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
            public static T Deserialize<T>(ReadOnlySpan<char> json, ISerdeJsonFormatter<T> formatter)
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
            public static T Deserialize<T>(ReadOnlySpan<char> json, IFixedSerdeJsonFormatter<T> formatter)
            {
                if (TrimWhiteSpace(json, out var jsonTrimmedWhiteSpace))
                {
                    return default;
                }

                jsonTrimmedWhiteSpace = JsonBufferSentinel.PadUtf16(jsonTrimmedWhiteSpace);
                return new JsonReaderUtf16(jsonTrimmedWhiteSpace).ReadValue(formatter);
            }

            #endregion

            /// <summary>
            /// Trims the leading whitespace characters
            /// </summary>
            /// <param name="origin">The original JSON buffer</param>
            /// <param name="trimmed">The buffer without its leading whitespace characters</param>
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
                    if (!StringHelper.IsWhiteSpace(origin[i]))
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
        /// UTF-8 JSON serializer/deserializer
        /// </summary>
        public static class UTF8
        {
            static UTF8() => ReusableBufferWriter<byte>.cachePool = ArrayPool.UTF8;

            #region Serialize Standard

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

            #endregion

            #region Serialize Stream

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="Stream"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="stream">The stream to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            public static void Serialize<T>(T value, Stream stream)
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
                    var buffer = bufferWriter.GetWrittenSpan();
                    stream.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="Stream"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="stream">The stream to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            public static void Serialize<T>(T value, ISerdeJsonFormatter<T> formatter, Stream stream)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    stream.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="Stream"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="stream">The stream to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            public static void Serialize<T>(T value, IFixedSerdeJsonFormatter<T> formatter, Stream stream)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    stream.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Asynchronously serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="Stream"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="stream">The stream to write the serialized JSON to</param>
            /// <param name="cancellationToken">The token to cancel the asynchronous write operation</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>A task that represents the asynchronous write operation</returns>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single asynchronous
            /// write call. Cancelling the token or failing the write can leave a partially written document in the target,
            /// nothing is rolled back, so the caller has to discard or rebuild it
            /// </remarks>
            public static async ValueTask SerializeAsync<T>(T value, Stream stream, CancellationToken cancellationToken = default)
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
                    var buffer = bufferWriter.GetWrittenMemory();
                    await stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Asynchronously serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="Stream"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="stream">The stream to write the serialized JSON to</param>
            /// <param name="cancellationToken">The token to cancel the asynchronous write operation</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>A task that represents the asynchronous write operation</returns>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single asynchronous
            /// write call. Cancelling the token or failing the write can leave a partially written document in the target,
            /// nothing is rolled back, so the caller has to discard or rebuild it
            /// </remarks>
            public static async ValueTask SerializeAsync<T>(T value, ISerdeJsonFormatter<T> formatter, Stream stream, CancellationToken cancellationToken = default)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenMemory();
                    await stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Asynchronously serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="Stream"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="stream">The stream to write the serialized JSON to</param>
            /// <param name="cancellationToken">The token to cancel the asynchronous write operation</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>A task that represents the asynchronous write operation</returns>
            /// <remarks>
            /// The whole document is serialized into a pooled buffer first and then written with a single asynchronous
            /// write call. Cancelling the token or failing the write can leave a partially written document in the target,
            /// nothing is rolled back, so the caller has to discard or rebuild it
            /// </remarks>
            public static async ValueTask SerializeAsync<T>(T value, IFixedSerdeJsonFormatter<T> formatter, Stream stream, CancellationToken cancellationToken = default)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenMemory();
                    await stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            #endregion

            #region Serialize BufferWriter

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="IBufferWriter{T}"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="writer">The buffer writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            public static void Serialize<T>(T value, IBufferWriter<byte> writer)
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
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="IBufferWriter{T}"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The buffer writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            public static void Serialize<T>(T value, ISerdeJsonFormatter<T> formatter, IBufferWriter<byte> writer)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="IBufferWriter{T}"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The buffer writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            public static void Serialize<T>(T value, IFixedSerdeJsonFormatter<T> formatter, IBufferWriter<byte> writer)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            #endregion

            #region Serialize BinaryWriter

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="BinaryWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="writer">The binary writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            /// <remarks>
            /// The raw UTF-8 bytes are appended with <c>BinaryWriter.Write(ReadOnlySpan&lt;byte&gt;)</c>; no byte order mark
            /// is written and <see cref="BinaryWriter.Encoding"/> is not involved
            /// </remarks>
            public static void Serialize<T>(T value, BinaryWriter writer)
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
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="BinaryWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The binary writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <remarks>
            /// The raw UTF-8 bytes are appended with <c>BinaryWriter.Write(ReadOnlySpan&lt;byte&gt;)</c>; no byte order mark
            /// is written and <see cref="BinaryWriter.Encoding"/> is not involved
            /// </remarks>
            public static void Serialize<T>(T value, ISerdeJsonFormatter<T> formatter, BinaryWriter writer)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            /// <summary>
            /// Serializes an object into UTF-8 encoded JSON and writes it to the specified <see cref="BinaryWriter"/>
            /// </summary>
            /// <param name="value">The value to serialize</param>
            /// <param name="formatter">The formatter</param>
            /// <param name="writer">The binary writer to write the serialized JSON to</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <remarks>
            /// The raw UTF-8 bytes are appended with <c>BinaryWriter.Write(ReadOnlySpan&lt;byte&gt;)</c>; no byte order mark
            /// is written and <see cref="BinaryWriter.Encoding"/> is not involved
            /// </remarks>
            public static void Serialize<T>(T value, IFixedSerdeJsonFormatter<T> formatter, BinaryWriter writer)
            {
                var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
                try
                {
                    var jsonWriter = new JsonWriterUtf8(bufferWriter);
                    jsonWriter.WriteValue(value, formatter);
                    jsonWriter.Flush();
                    var buffer = bufferWriter.GetWrittenSpan();
                    writer.Write(buffer);
                }
                finally
                {
                    ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
                }
            }

            #endregion

            #region Deserialize Standard

            /// <summary>
            /// Deserializes a UTF-8 string into an object
            /// </summary>
            /// <param name="json">The UTF-8 string</param>
            /// <typeparam name="T">The type of the value to serialize</typeparam>
            /// <returns>The deserialized object instance</returns>
            /// <exception cref="SerdeException">Thrown when no formatter has ever been registered for the type</exception>
            public static T Deserialize<T>(ReadOnlySpan<byte> json)
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
            public static T Deserialize<T>(ReadOnlySpan<byte> json, ISerdeJsonFormatter<T> formatter)
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
            public static T Deserialize<T>(ReadOnlySpan<byte> json, IFixedSerdeJsonFormatter<T> formatter)
            {
                if (TrimWhiteSpace(json, out var jsonTrimmedWhiteSpace))
                {
                    return default;
                }

                jsonTrimmedWhiteSpace = JsonBufferSentinel.PadUtf8(jsonTrimmedWhiteSpace);
                return new JsonReaderUtf8(jsonTrimmedWhiteSpace).ReadValue(formatter);
            }

            #endregion

            /// <summary>
            /// Trims the leading whitespace characters
            /// </summary>
            /// <param name="origin">The original JSON buffer</param>
            /// <param name="trimmed">The buffer without its leading whitespace characters</param>
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
                    if (!StringHelper.IsWhiteSpace(origin[i]))
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
            /// <summary>
            /// The formatter instance for the built-in generic type, or null when the type is not supported
            /// </summary>
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
                    // An enum has no element type, so the generic argument is the enum type itself
                    var type = typeof(EnumNumberFormatter<>).MakeGenericType(typeof(T));
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

            /// <summary>
            /// Checks whether the type implements a generic collection interface
            /// </summary>
            /// <param name="type">The type to check</param>
            /// <returns>True if the type is a collection, otherwise false</returns>
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

            /// <summary>
            /// Checks whether the type is <see cref="Nullable{T}"/>
            /// </summary>
            /// <param name="type">The type to check</param>
            /// <returns>True if the type is nullable, otherwise false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsNullable(Type type) =>
                type.Name.Equals("Nullable`1") && type.Namespace.Equals("System");

            /// <summary>
            /// Checks whether the type is <see cref="Lazy{T}"/>
            /// </summary>
            /// <param name="type">The type to check</param>
            /// <returns>True if the type is lazy, otherwise false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsLazy(Type type) =>
                type.Name.Equals("Lazy`1") && type.Namespace.Equals("System");

            /// <summary>
            /// Checks whether the type is <see cref="KeyValuePair{TKey,TValue}"/>
            /// </summary>
            /// <param name="type">The type to check</param>
            /// <returns>True if the type is a key-value pair, otherwise false</returns>
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
file static class JsonBufferSentinel
{
    /// <summary>
    /// Checks whether the given last byte needs a trailing space to terminate the value
    /// </summary>
    /// <param name="last">The last byte of the JSON buffer</param>
    /// <returns>True if the buffer needs padding, otherwise false</returns>
    internal static bool NeedsPadding(byte last) =>
        last != (byte) '}' && last != (byte) ']' && last != (byte) ',' && last != (byte) '"'
        && !PeroTools2.Serde.StringHelper.IsWhiteSpace(last);

    /// <summary>
    /// Checks whether the given last character needs a trailing space to terminate the value
    /// </summary>
    /// <param name="last">The last character of the JSON buffer</param>
    /// <returns>True if the buffer needs padding, otherwise false</returns>
    internal static bool NeedsPadding(char last) =>
        last != '}' && last != ']' && last != ',' && last != '"'
        && !PeroTools2.Serde.StringHelper.IsWhiteSpace(last);

    /// <summary>
    /// Appends a trailing space to the UTF-8 JSON buffer when required
    /// </summary>
    /// <param name="json">The UTF-8 JSON buffer</param>
    /// <returns>The padded buffer, or the original buffer when no padding is needed</returns>
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

    /// <summary>
    /// Appends a trailing space to the UTF-16 JSON buffer when required
    /// </summary>
    /// <param name="json">The UTF-16 JSON buffer</param>
    /// <returns>The padded buffer, or the original buffer when no padding is needed</returns>
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