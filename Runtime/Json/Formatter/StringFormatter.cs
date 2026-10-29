/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 字符串 Json 格式化器
    /// </summary>
    [PublicAPI]
    public sealed class StringFormatter : ISerdeJsonFormatter<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(string value, ref JsonWriterUtf8 writer) => writer.WriteString(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(string value, ref JsonWriterUtf16 writer) => writer.WriteString(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string DeserializeUtf8(ref ReadOnlySpan<byte> span, ref JsonReaderUtf8 reader)
        {
            var remains = reader.remains;
            unsafe
            {
                fixed (byte* src = remains)
                {
                    // 与游标路径共用同一个字符串解析入口
                    var value = JsonReader.ReadStringValueUtf8(src, remains.Length, out var advance);
                    reader.Advance(advance);
                    return value;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string DeserializeUtf16(ref ReadOnlySpan<char> span, ref JsonReaderUtf16 reader)
        {
            var remains = reader.remains;
            unsafe
            {
                fixed (char* src = remains)
                {
                    // 与游标路径共用同一个字符串解析入口
                    var value = JsonReader.ReadStringValueUtf16(src, remains.Length, out var advance);
                    reader.Advance(advance);
                    return value;
                }
            }
        }
    }
}