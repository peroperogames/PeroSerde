/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Description: 供生成代码直接调用的基础类型读取器（P1 原型）
 */

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 基础类型的直接读取入口。
    /// 生成代码直接调用这些静态方法，跳过 <see cref="SerdeJsonFormatterProvider"/>
    /// 的每值查找与 <c>ISerdeJsonFormatter</c> 的接口分发。
    /// </summary>
    public static unsafe class JsonPrimitives
    {
        /// <summary>读取嵌套的 [SerdeJsonSerializable] 对象：借道 reader，把消费量同步回游标。</summary>
        public static T ReadNested<T>(ref JsonCursor cursor)
        {
            var span = new ReadOnlySpan<byte>(cursor.p, (int) (cursor.end - cursor.p));
            var reader = new JsonReaderUtf8(span, cursor.p);
            // 交给 reader 处理 fixed / unfixed 两种 formatter 形态
            var value = reader.ReadValue<T>(false);
            cursor.p = (byte*) (cursor.end - reader.remains.Length);
            return value;
        }

        /// <summary>用成员指定的 formatter 读取（显式 [SerdeJsonFormatter] 的类型不在 provider 注册表里）。</summary>
        public static T ReadNested<T>(ref JsonCursor cursor, object formatter)
        {
            var span = new ReadOnlySpan<byte>(cursor.p, (int) (cursor.end - cursor.p));
            var reader = new JsonReaderUtf8(span, cursor.p);
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

            cursor.p = (byte*) (cursor.end - reader.remains.Length);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToInt(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToLong(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToUint(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToUlong(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToShort(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToUshort(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToByte(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToSbyte(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToFloat(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToDouble(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToDecimal(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            var span = JsonReader.ReadPrimitiveUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToBool(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(ref JsonCursor cursor)
        {
            // char 在 JSON 里是带引号的单字符串（如 "a"），需要先按字符串读取再取第一个字符
            cursor.SkipWhiteSpace();
            if (cursor.p < cursor.end && *cursor.p == (byte) '"')
            {
                var remaining = (int) (cursor.end - cursor.p);
                if (JsonReader.TryReadAsciiStringUtf8(cursor.p, remaining, out var ascii, out var asciiAdvance))
                {
                    cursor.p += asciiAdvance;
                    return ascii.Length > 0 ? ascii[0] : '\0';
                }
            }

            var span = JsonReader.ReadValueUntilEndUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return SpanConverter.ToChar(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            if (cursor.p >= cursor.end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            var remaining = (int) (cursor.end - cursor.p);
            if (*cursor.p == (byte) '"')
            {
                if (JsonReader.TryReadAsciiStringUtf8(cursor.p, remaining, out var ascii, out var asciiAdvance))
                {
                    cursor.p += asciiAdvance;
                    return ascii;
                }

                // 非 ASCII 或非 ASCII 转义: 走通用路径解码
                var quoted = JsonReader.ReadValueUntilEndUtf8(cursor.p, remaining, out var quotedAdvance);
                cursor.p += quotedAdvance;
                return Encoding.UTF8.GetString(quoted);
            }

            // 非字符串 token: 只允许 null, 其余与旧路径一致直接报错
            var span = JsonReader.ReadValueUntilEndUtf8(cursor.p, remaining, out var advance);
            cursor.p += advance;
            if (SpanConverter.IsNull(span))
            {
                return null;
            }

            throw SerdeException.ThrowInvalidJson("Invalid json format.");
        }
    }
}