/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23-17:41:00
 */

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json;

/// <summary>
/// General-purpose formatter
/// </summary>
/// <typeparam name="T">Formatted type</typeparam>
public interface ISerdeJsonFormatter<T>
{
    /// <summary>
    /// Serializes to a UTF-8 string
    /// </summary>
    /// <param name="value">The value to serialize</param>
    /// <param name="writer">Json Utf8 writer</param>
    void SerializeUtf8(T value, ref JsonWriterUtf8 writer);

    /// <summary>
    /// Serializes to a UTF-16 string
    /// </summary>
    /// <param name="value">The value to serialize</param>
    /// <param name="writer">Json Utf16 writer</param>
    void SerializeUtf16(T value, ref JsonWriterUtf16 writer);

    /// <summary>
    /// Deserializes from a UTF-8 string
    /// </summary>
    /// <param name="remains">The remaining Json string</param>
    /// <param name="reader">Json Utf8 reader</param>
    /// <returns>The deserialized instance</returns>
    T DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader);

    /// <summary>
    /// Deserializes from a UTF-16 string
    /// </summary>
    /// <param name="remains">The remaining Json string</param>
    /// <param name="reader">Json Utf16 reader</param>
    /// <returns>The deserialized instance</returns>
    T DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader);
}

/// <summary>
/// Formatter for fixed-size types
/// </summary>
/// <typeparam name="T">Formatted type</typeparam>
public interface IFixedSerdeJsonFormatter<T>
{
    int GetMaxRequiredSizeUtf8();
    int GetMaxRequiredSizeUtf16();

    unsafe int SerializeUtf16Unsafe(T value, char* dest);
    unsafe int SerializeUtf8Unsafe(T value, byte* dest);

    unsafe T DeserializeUtf8Unsafe(byte* src, out int advance);
    unsafe T DeserializeUtf16Unsafe(char* src, out int advance);
}

/// <summary>
/// Encoding formatter for Primitive types
/// </summary>
/// <typeparam name="T">Formatted type; only Primitive types are supported</typeparam>
public abstract class EncodingJsonFormatter<T> : ISerdeJsonFormatter<T>
{
    static EncodingJsonFormatter()
    {
        if (Type.GetTypeCode(typeof(T)) == TypeCode.Object)
        {
            throw new SerdeException($"Formatter for type {typeof(T)} is not supported.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ISerdeJsonFormatter<T>.SerializeUtf8(T value, ref JsonWriterUtf8 writer) => writer.WriteValue(SerializeUtf8(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ISerdeJsonFormatter<T>.SerializeUtf16(T value, ref JsonWriterUtf16 writer) => writer.WriteValue(SerializeUtf16(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    T ISerdeJsonFormatter<T>.DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader) => DeserializeUtf8(reader.ReadValue());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    T ISerdeJsonFormatter<T>.DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader) => DeserializeUtf16(reader.ReadValue());

    /// <summary>
    /// Deserializes from a UTF-16 string
    /// </summary>
    /// <param name="value">The string to deserialize</param>
    /// <returns>The deserialized object instance</returns>
    public abstract T DeserializeUtf16(ReadOnlySpan<char> value);

    /// <summary>
    /// Deserializes from a UTF-8 string
    /// </summary>
    /// <param name="value">The string to deserialize</param>
    /// <returns>The deserialized object instance</returns>
    public abstract T DeserializeUtf8(ReadOnlySpan<byte> value);

    /// <summary>
    /// Serializes to a UTF-16 string
    /// </summary>
    /// <param name="value">The object instance to serialize</param>
    /// <returns>The serialized UTF-16 string</returns>
    public abstract ReadOnlySpan<char> SerializeUtf16(T value);

    /// <summary>
    /// Serializes to a UTF-16 string
    /// </summary>
    /// <param name="value">The object instance to serialize</param>
    /// <returns>The serialized UTF-8 string</returns>
    public abstract ReadOnlySpan<byte> SerializeUtf8(T value);
}

/// <summary>
/// Primitive Json formatter
/// </summary>
/// <typeparam name="T">Formatted type; only Primitive types are supported</typeparam>
public abstract class PrimitiveJsonFormatter<T> : ISerdeJsonFormatter<T>
{
    static PrimitiveJsonFormatter()
    {
        if (Type.GetTypeCode(typeof(T)) == TypeCode.Object)
        {
            throw new SerdeException($"Formatter for type {typeof(T)} is not supported.");
        }
    }

    /// <summary>
    /// Upper bound on the bytes allocated on the stack; above it the buffer is rented from the pool to avoid a stack overflow.
    /// Both directions compare the actual stack bytes used so that stack usage stays consistent.
    /// </summary>
    private const int max_stack_alloc_bytes = 1024;

    protected abstract T Deserialize(ReadOnlySpan<char> value);
    protected abstract ReadOnlySpan<char> Serialize(T value);

    void ISerdeJsonFormatter<T>.SerializeUtf16(T value, ref JsonWriterUtf16 writer) => writer.WriteValue(Serialize(value));
    T ISerdeJsonFormatter<T>.DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader) => Deserialize(reader.ReadValue());

    unsafe void ISerdeJsonFormatter<T>.SerializeUtf8(T value, ref JsonWriterUtf8 writer)
    {
        var utf16 = Serialize(value);
        var maxByteCount = Encoding.UTF8.GetMaxByteCount(utf16.Length);
        // The stack buffer holds bytes, so compare byte counts directly
        var canAllocateOnStack = maxByteCount < max_stack_alloc_bytes;
        var array = canAllocateOnStack ? Array.Empty<byte>() : ArrayPool.UTF8.Rent(maxByteCount);
        var utf8 = canAllocateOnStack ? stackalloc byte[maxByteCount] : array;
        try
        {
            var charCount = Encoding.UTF8.GetBytes(utf16, utf8);
            fixed (byte* utf8Ptr = utf8)
            {
                writer.WriteValue(new ReadOnlySpan<byte>(utf8Ptr, charCount));
            }
        }
        finally
        {
            if (!canAllocateOnStack) ArrayPool.UTF8.Return(array);
        }
    }

    T ISerdeJsonFormatter<T>.DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
    {
        var value = reader.ReadValue();
        var maxCharCount = Encoding.UTF8.GetMaxCharCount(value.Length);
        // The stack buffer holds chars, so convert to a byte count before comparing with the limit
        var canAllocateOnStack = maxCharCount * sizeof(char) < max_stack_alloc_bytes;
        var array = canAllocateOnStack ? Array.Empty<char>() : ArrayPool.UTF16.Rent(maxCharCount);
        var utf16 = canAllocateOnStack ? stackalloc char[maxCharCount] : array;
        try
        {
            var charCount = Encoding.UTF8.GetChars(value, utf16);
            return Deserialize(utf16.Slice(0, charCount));
        }
        finally
        {
            if (!canAllocateOnStack) ArrayPool.UTF16.Return(array);
        }
    }
}