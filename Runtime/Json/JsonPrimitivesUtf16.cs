/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Description: 供生成代码直接调用的 UTF-16 基础类型读取器（P1 原型）
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// UTF-16 版的基础类型直读入口，与 <see cref="JsonPrimitives"/> 一一对应。
    /// </summary>
    public static unsafe class JsonPrimitivesUtf16
    {
        /// <summary>读取嵌套的 [SerdeJsonSerializable] 对象：借道 reader，把消费量同步回游标。</summary>
        public static T ReadNested<T>(ref JsonCursorUtf16 cursor)
        {
            var span = new ReadOnlySpan<char>(cursor.p, (int) (cursor.end - cursor.p));
            var reader = new JsonReaderUtf16(span, cursor.p);
            var value = reader.ReadValue<T>(false);
            cursor.p = (char*) (cursor.end - reader.remains.Length);
            return value;
        }

        /// <summary>用成员指定的 formatter 读取（显式 [SerdeJsonFormatter] 的类型不在 provider 注册表里）。</summary>
        public static T ReadNested<T>(ref JsonCursorUtf16 cursor, object formatter)
        {
            var span = new ReadOnlySpan<char>(cursor.p, (int) (cursor.end - cursor.p));
            var reader = new JsonReaderUtf16(span, cursor.p);
            T value;
            if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
            {
                value = reader.ReadValue(fixedFormatter, false);
            }
            else if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
            {
                value = reader.ReadValue(unfixedFormatter, false);
            }
            else
            {
                throw SerdeException.ThrowFormatterNotFound(typeof(T));
            }

            cursor.p = (char*) (cursor.end - reader.remains.Length);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToInt(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToLong(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToUint(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToUlong(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToShort(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToUshort(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToByte(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToSbyte(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToFloat(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToDouble(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToDecimal(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(ref JsonCursorUtf16 cursor)
        {
            var span = ReadPrimitive(ref cursor, out var advance);
            cursor.p += advance;
            return SpanConverter.ToBool(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(ref JsonCursorUtf16 cursor)
        {
            var remaining = BeginPrimitive(ref cursor);
            if (remaining[0] == '"')
            {
                var quoted = JsonReader.ReadValueUntilEndUtf16(cursor.p, remaining.Length, out var quotedAdvance);
                cursor.p += quotedAdvance;
                return quoted.Length > 0 ? SpanConverter.ToChar(quoted) : '\0';
            }

            var span = JsonReader.ReadValueUntilEndUtf16(cursor.p, remaining.Length, out var advance);
            cursor.p += advance;
            return SpanConverter.ToChar(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(ref JsonCursorUtf16 cursor)
        {
            var remaining = BeginPrimitive(ref cursor);
            if (remaining[0] == '"')
            {
                var quoted = JsonReader.ReadValueUntilEndUtf16(cursor.p, remaining.Length, out var quotedAdvance);
                cursor.p += quotedAdvance;
                return SpanConverter.ToString(quoted);
            }

            // 非字符串 token: 只允许 null, 其余与旧路径一致直接报错
            var span = JsonReader.ReadValueUntilEndUtf16(cursor.p, remaining.Length, out var advance);
            cursor.p += advance;
            if (SpanConverter.IsNull(span))
            {
                return null;
            }

            throw SerdeException.ThrowInvalidJson("Invalid json format.");
        }

        /// <summary>读出当前数值 token 的原始区间。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<char> ReadPrimitive(ref JsonCursorUtf16 cursor, out int advance)
        {
            var remaining = BeginPrimitive(ref cursor);
            return JsonReader.ReadPrimitiveUntilEndUtf16(cursor.p, remaining.Length, out advance);
        }

        /// <summary>跳过空白并返回剩余区间；空输入直接抛错。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<char> BeginPrimitive(ref JsonCursorUtf16 cursor)
        {
            cursor.SkipWhiteSpace();
            if (cursor.p >= cursor.end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            return new ReadOnlySpan<char>(cursor.p, (int) (cursor.end - cursor.p));
        }
    }
}
