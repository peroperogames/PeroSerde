/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// Newtonsoft.Json 风格的 Dictionary 格式化器
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    [PublicAPI]
    public sealed class DictionaryFormatter<TKey, TValue> : DictionaryFormatter<Dictionary<TKey, TValue>, TKey, TValue> { }

    /// <summary>
    /// Newtonsoft.Json 风格的 Dictionary 格式化器
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <typeparam name="TDictionary">字典类型</typeparam>
    public class DictionaryFormatter<TDictionary, TKey, TValue> : ISerdeJsonFormatter<TDictionary> where TDictionary : IDictionary<TKey, TValue>, new()
    {
        private static readonly EncodingJsonFormatter<TKey> s_KeyEncodingJsonFormatter = SerdeJsonFormatterProvider.GetFormatterInternal<TKey>() as EncodingJsonFormatter<TKey>;
        private static readonly IFixedSerdeJsonFormatter<TKey> s_KeyFixedFormatter = SerdeJsonFormatterProvider.GetFormatterInternal<TKey>() as IFixedSerdeJsonFormatter<TKey>;
        private static readonly TypeCode s_KeyType = Type.GetTypeCode(typeof(TKey));

        public unsafe void SerializeUtf8(TDictionary dic, ref JsonWriterUtf8 writer)
        {
            if (dic == null)
            {
                writer.WriteNull();
                return;
            }
            var index = 0;
            var count = dic.Count - 1;
            writer.WriteBeginObject();

            if (s_KeyFixedFormatter == null)
            {
                foreach (var kv in dic)
                {
                    if (kv.Key is string key)
                    {
                        writer.WriteNamedValue(key, kv.Value, index++ != count);
                    }
                    else
                    {
                        writer.WriteNamedValue(kv.Key.ToString(), kv.Value, index++ != count);
                    }
                }
            }
            else
            {
                var maxLength = s_KeyFixedFormatter.GetMaxRequiredSizeUtf8();
                var result = stackalloc byte[maxLength];
                var chars = stackalloc char[maxLength];
                foreach (var kv in dic)
                {
                    var length = s_KeyFixedFormatter.SerializeUtf8Unsafe(kv.Key, result);
                    for (var i = 0; i < length; i++)
                    {
                        chars[i] = (char) result[i];
                    }
                    writer.WriteNamedValue(new ReadOnlySpan<char>(chars, length), kv.Value, index++ != count);
                }
            }
            writer.WriteEndObject();
        }

        public unsafe void SerializeUtf16(TDictionary dic, ref JsonWriterUtf16 writer)
        {
            if (dic == null)
            {
                writer.WriteNull();
                return;
            }
            var index = 0;
            var count = dic.Count - 1;

            writer.WriteBeginObject();
            if (s_KeyFixedFormatter == null)
            {
                foreach (var kv in dic)
                {
                    if (kv.Key is string key)
                    {
                        writer.WriteNamedValue(key, kv.Value, index++ != count);
                    }
                    else
                    {
                        writer.WriteNamedValue(kv.Key.ToString(), kv.Value, index++ != count);
                    }
                }
            }
            else
            {
                var maxLength = s_KeyFixedFormatter.GetMaxRequiredSizeUtf16();
                var result = stackalloc char[maxLength];
                foreach (var kv in dic)
                {
                    var length = s_KeyFixedFormatter.SerializeUtf16Unsafe(kv.Key, result);
                    writer.WriteNamedValue(new ReadOnlySpan<char>(result, length), kv.Value, index++ != count);
                }
            }
            writer.WriteEndObject();
        }

        public TDictionary DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            if (remains.Length >= 4 && SpanConverter.IsNull(remains.Slice(0, 4)))
            {
                return default;
            }
            var dic = new TDictionary();
            reader.ReadBeginObject();
            while (reader.remains[0] != '}')
            {
                dic.Add(ReadKeyValue(ref reader));
                if (reader.remains[0] == ',') reader.ReadComma();
            }
            reader.ReadEndObject();
            return dic;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static KeyValuePair<TKey, TValue> ReadKeyValue(ref JsonReaderUtf8 reader)
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
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static unsafe TKey DeserializeKeyFromFormatter(ref ReadOnlySpan<byte> keyStr)
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

        public TDictionary DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            if (remains.Length >= 4 && SpanConverter.IsNull(remains.Slice(0, 4)))
            {
                return default;
            }
            var dic = new TDictionary();
            reader.ReadBeginObject();

            while (reader.remains[0] != '}')
            {
                dic.Add(ReadKeyValue(ref reader));
                if (reader.remains[0] == ',') reader.ReadComma();
            }
            reader.ReadEndObject();
            return dic;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static KeyValuePair<TKey, TValue> ReadKeyValue(ref JsonReaderUtf16 reader)
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
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static unsafe TKey DeserializeKeyFromFormatter(ref ReadOnlySpan<char> keyStr)
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
        private static TKey CastKey<TFrom>(TFrom value) => Unsafe.As<TFrom, TKey>(ref value);
    }
}