/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23-17:41:00
 */

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 通用格式化器
    /// </summary>
    /// <typeparam name="T">格式化对象类型</typeparam>
    public interface ISerdeJsonFormatter<T>
    {
        /// <summary>
        /// 序列化 UTF-8 字符串
        /// </summary>
        /// <param name="value">当前需要被序列化的值</param>
        /// <param name="writer">Json Utf8 写入器</param>
        void SerializeUtf8(T value, ref JsonWriterUtf8 writer);

        /// <summary>
        /// 序列化 UTF-16 字符串
        /// </summary>
        /// <param name="value">当前需要被序列化的值</param>
        /// <param name="writer">Json Utf16 写入器</param>
        void SerializeUtf16(T value, ref JsonWriterUtf16 writer);

        /// <summary>
        /// 反序列化 UTF-8 字符串
        /// </summary>
        /// <param name="remains">剩余的 Json 字符串</param>
        /// <param name="reader">Json Utf8 读取器</param>
        /// <returns>反序列化后的实例</returns>
        T DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader);

        /// <summary>
        /// 反序列化 UTF-16 字符串
        /// </summary>
        /// <param name="remains">剩余的 Json 字符串</param>
        /// <param name="reader">Json Utf16 读取器</param>
        /// <returns>反序列化后的实例</returns>
        T DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader);
    }

    /// <summary>
    /// 定长类型的格式化器
    /// </summary>
    /// <typeparam name="T">格式化对象类型</typeparam>
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
    /// Primitive 的编码格式化器
    /// </summary>
    /// <typeparam name="T">格式化对象类型，仅支持 Primitive 类型</typeparam>
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
        /// 反序列化 UTF-16 字符串
        /// </summary>
        /// <param name="value">需要反序列化的字符串</param>
        /// <returns>反序列化后的对象实例</returns>
        public abstract T DeserializeUtf16(ReadOnlySpan<char> value);

        /// <summary>
        /// 反序列化 UTF-8 字符串
        /// </summary>
        /// <param name="value">需要反序列化的字符串</param>
        /// <returns>反序列化后的对象实例</returns>
        public abstract T DeserializeUtf8(ReadOnlySpan<byte> value);

        /// <summary>
        /// 序列化 UTF-16 字符串
        /// </summary>
        /// <param name="value">序列化的对象的实例</param>
        /// <returns>序列化后的 UTF-16 字符串</returns>
        public abstract ReadOnlySpan<char> SerializeUtf16(T value);

        /// <summary>
        /// 序列化 UTF-16 字符串
        /// </summary>
        /// <param name="value">序列化的对象的实例</param>
        /// <returns>序列化后的 UTF-8 字符串</returns>
        public abstract ReadOnlySpan<byte> SerializeUtf8(T value);
    }

    /// <summary>
    /// Primitive Json 格式化器
    /// </summary>
    /// <typeparam name="T">格式化对象类型，仅支持 Primitive 类型</typeparam>
    public abstract class PrimitiveJsonFormatter<T> : ISerdeJsonFormatter<T>
    {
        static PrimitiveJsonFormatter()
        {
            if (Type.GetTypeCode(typeof(T)) == TypeCode.Object)
            {
                throw new SerdeException($"Formatter for type {typeof(T)} is not supported.");
            }
        }

        protected abstract T Deserialize(ReadOnlySpan<char> value);
        protected abstract ReadOnlySpan<char> Serialize(T value);

        void ISerdeJsonFormatter<T>.SerializeUtf16(T value, ref JsonWriterUtf16 writer) => writer.WriteValue(Serialize(value));
        T ISerdeJsonFormatter<T>.DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader) => Deserialize(reader.ReadValue());

        unsafe void ISerdeJsonFormatter<T>.SerializeUtf8(T value, ref JsonWriterUtf8 writer)
        {
            var utf16 = Serialize(value);
            Span<byte> utf8 = stackalloc byte[Encoding.UTF8.GetMaxByteCount(utf16.Length)];
            var charCount = Encoding.UTF8.GetBytes(utf16, utf8);
            fixed (byte* utf8Ptr = utf8)
            {
                writer.WriteValue(new ReadOnlySpan<byte>(utf8Ptr, charCount));
            }
        }

        T ISerdeJsonFormatter<T>.DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var value = reader.ReadValue();
            Span<char> utf16 = stackalloc char[Encoding.UTF8.GetMaxCharCount(value.Length)];
            var charCount = Encoding.UTF8.GetChars(value, utf16);
            return Deserialize(utf16.Slice(0, charCount));
        }
    }
}