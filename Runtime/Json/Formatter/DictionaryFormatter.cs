/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Collections.Generic;
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
        public void SerializeUtf8(TDictionary dic, ref JsonWriterUtf8 writer)
        {
            if (dic == null)
            {
                writer.WriteNull();
                return;
            }
            var index = 0;
            var count = dic.Count - 1;
            writer.WriteBeginObject();
            foreach (var kv in dic)
            {
                writer.WriteNamedValue(kv.Key.ToString(), kv.Value, index++ != count);
            }
            writer.WriteEndObject();
        }

        public void SerializeUtf16(TDictionary dic, ref JsonWriterUtf16 writer)
        {
            if (dic == null)
            {
                writer.WriteNull();
                return;
            }
            var index = 0;
            var count = dic.Count - 1;

            writer.WriteBeginObject();
            foreach (var kv in dic)
            {
                writer.WriteNamedValue(kv.Key.ToString(), kv.Value, index++ != count);
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
                dic.Add(reader.ReadValue<KeyValuePair<TKey, TValue>>());
                if (reader.remains[0] == ',') reader.ReadComma();
            }
            reader.ReadEndObject();
            return dic;
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
                dic.Add(reader.ReadValue<KeyValuePair<TKey, TValue>>());
                if (reader.remains[0] == ',') reader.ReadComma();
            }
            reader.ReadEndObject();
            return dic;
        }
    }
}