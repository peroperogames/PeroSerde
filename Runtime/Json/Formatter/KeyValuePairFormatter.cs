/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/07-14:55:24
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 写入或读取 Json 键值对的格式化器
    /// </summary>
    /// <typeparam name="TKey">Key 类型</typeparam>
    /// <typeparam name="TValue">Value 类型</typeparam>
    [PublicAPI]
    public sealed class KeyValuePairFormatter<TKey, TValue> : ISerdeJsonFormatter<KeyValuePair<TKey, TValue>>
    {
        private static readonly EncodingJsonFormatter<TKey> s_KeyEncodingJsonFormatter = SerdeJsonFormatterProvider.GetFormatterInternal<TKey>() as EncodingJsonFormatter<TKey>;
        private static readonly IFixedSerdeJsonFormatter<TKey> s_KeyFixedFormatter = SerdeJsonFormatterProvider.GetFormatterInternal<TKey>() as IFixedSerdeJsonFormatter<TKey>;
        private static readonly TypeCode s_KeyType = Type.GetTypeCode(typeof(TKey));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SerializeUtf8(KeyValuePair<TKey, TValue> kv, ref JsonWriterUtf8 writer)
        {
            writer.WriteBeginObject();
            if (s_KeyFixedFormatter == null)
            {
                if (kv.Key is string key)
                {
                    writer.WriteNamedValue(key, kv.Value);
                }
                else
                {
                    writer.WriteNamedValue(kv.Key.ToString(), kv.Value);
                }
            }
            else
            {
                var maxLength = s_KeyFixedFormatter.GetMaxRequiredSizeUtf8();
                var result = stackalloc byte[maxLength];
                var length = s_KeyFixedFormatter.SerializeUtf8Unsafe(kv.Key, result);
                writer.WriteNamedValue(MemoryMarshal.Cast<byte, char>(new ReadOnlySpan<byte>(result, length)), kv.Value);
            }
            writer.WriteNamedValue(kv.Key.ToString(), kv.Value);
            writer.WriteEndObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SerializeUtf16(KeyValuePair<TKey, TValue> kv, ref JsonWriterUtf16 writer)
        {
            writer.WriteBeginObject();
            if (s_KeyFixedFormatter == null)
            {
                if (kv.Key is string key)
                {
                    writer.WriteNamedValue(key, kv.Value);
                }
                else
                {
                    writer.WriteNamedValue(kv.Key.ToString(), kv.Value);
                }
            }
            else
            {
                var maxLength = s_KeyFixedFormatter.GetMaxRequiredSizeUtf16();
                var result = stackalloc char[maxLength];
                var length = s_KeyFixedFormatter.SerializeUtf16Unsafe(kv.Key, result);
                writer.WriteNamedValue(new ReadOnlySpan<char>(result, length), kv.Value);
            }
            writer.WriteEndObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe KeyValuePair<TKey, TValue> DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var keyStr = reader.ReadKey();
            var value = reader.ReadValue<TValue>();
            return s_KeyType switch
            {
                TypeCode.Empty => throw SerdeException.ThrowNotSupportedType(typeof(TKey)),
                TypeCode.DBNull => throw SerdeException.ThrowNotSupportedType(typeof(TKey)),
                TypeCode.Boolean => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToBool(keyStr)), value),
                TypeCode.Byte => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToByte(keyStr)), value),
                TypeCode.SByte => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToSbyte(keyStr)), value),
                TypeCode.Char => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToChar(keyStr)), value),
                TypeCode.DateTime => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToDateTime(keyStr)), value),
                TypeCode.Decimal => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToDecimal(keyStr)), value),
                TypeCode.Double => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToDouble(keyStr)), value),
                TypeCode.Single => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToFloat(keyStr)), value),
                TypeCode.Int16 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToShort(keyStr)), value),
                TypeCode.Int32 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToInt(keyStr)), value),
                TypeCode.Int64 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToLong(keyStr)), value),
                TypeCode.String => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToString(keyStr)), value),
                TypeCode.UInt16 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToUshort(keyStr)), value),
                TypeCode.UInt32 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToUint(keyStr)), value),
                TypeCode.UInt64 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToUlong(keyStr)), value),
                TypeCode.Object => new KeyValuePair<TKey, TValue>(DeserializeKeyFromFormatter(ref keyStr), value),
                _ => throw new ArgumentOutOfRangeException()
            };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static TKey DeserializeKeyFromFormatter(ref ReadOnlySpan<byte> keyStr)
            {
                if (s_KeyFixedFormatter != null)
                {
                    var length = keyStr.Length + 1;
                    var result = stackalloc byte[length];
                    result[length - 1] = (byte) ',';
                    var span = new Span<byte>(result, keyStr.Length);
                    keyStr.CopyTo(span);
                    return s_KeyFixedFormatter.DeserializeUtf8Unsafe(result, out _);
                }
                if (s_KeyEncodingJsonFormatter != null)
                {
                    return s_KeyEncodingJsonFormatter.DeserializeUtf8(keyStr);
                }
                if (typeof(TKey) == typeof(string))
                {
                    var key = SpanConverter.ToString(keyStr);
                    return Unsafe.As<string, TKey>(ref key);
                }
                throw new SerdeException("None formatter found for key.Type: " + typeof(TKey).Name);
            }
        }

        public unsafe KeyValuePair<TKey, TValue> DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            var keyStr = reader.ReadKey();
            var value = reader.ReadValue<TValue>();
            return s_KeyType switch
            {
                TypeCode.Empty => throw SerdeException.ThrowNotSupportedType(typeof(TKey)),
                TypeCode.DBNull => throw SerdeException.ThrowNotSupportedType(typeof(TKey)),
                TypeCode.Boolean => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToBool(keyStr)), value),
                TypeCode.Byte => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToByte(keyStr)), value),
                TypeCode.SByte => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToSbyte(keyStr)), value),
                TypeCode.Char => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToChar(keyStr)), value),
                TypeCode.DateTime => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToDateTime(keyStr)), value),
                TypeCode.Decimal => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToDecimal(keyStr)), value),
                TypeCode.Double => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToDouble(keyStr)), value),
                TypeCode.Single => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToFloat(keyStr)), value),
                TypeCode.Int16 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToShort(keyStr)), value),
                TypeCode.Int32 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToInt(keyStr)), value),
                TypeCode.Int64 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToLong(keyStr)), value),
                TypeCode.String => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToString(keyStr)), value),
                TypeCode.UInt16 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToUshort(keyStr)), value),
                TypeCode.UInt32 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToUint(keyStr)), value),
                TypeCode.UInt64 => new KeyValuePair<TKey, TValue>(CastKey(SpanConverter.ToUlong(keyStr)), value),
                TypeCode.Object => new KeyValuePair<TKey, TValue>(DeserializeKeyFromFormatter(ref keyStr), value),
                _ => throw new ArgumentOutOfRangeException()
            };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static TKey DeserializeKeyFromFormatter(ref ReadOnlySpan<char> keyStr)
            {
                if (s_KeyFixedFormatter != null)
                {
                    var length = keyStr.Length + 1;
                    var result = stackalloc char[length];
                    result[length - 1] = ',';
                    var span = new Span<char>(result, keyStr.Length);
                    keyStr.CopyTo(span);
                    return s_KeyFixedFormatter.DeserializeUtf16Unsafe(result, out _);
                }
                if (s_KeyEncodingJsonFormatter != null)
                {
                    return s_KeyEncodingJsonFormatter.DeserializeUtf16(keyStr);
                }
                if (typeof(TKey) == typeof(string))
                {
                    var key = SpanConverter.ToString(keyStr);
                    return Unsafe.As<string, TKey>(ref key);
                }
                throw new SerdeException("None formatter found for key.Type: " + typeof(TKey).Name);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TKey CastKey<TFrom>(TFrom value) => Unsafe.As<TFrom, TKey>(ref value);
    }
}