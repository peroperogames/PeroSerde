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
    /// 解析逻辑同样只有一份实现：全部转发给 <see cref="JsonReader"/> 的 <c>ReadXxxUtf16</c>。
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
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadIntUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadLongUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadUintUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadUlongUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadShortUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadUshortUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadByteUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadSbyteUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadFloatUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadDoubleUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadDecimalUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadBoolUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadCharUtf16(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(ref JsonCursorUtf16 cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadStringValueUtf16(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return value;
        }

        /// <summary>跳过空白并确认还有内容可读。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BeginPrimitive(ref JsonCursorUtf16 cursor)
        {
            cursor.SkipWhiteSpace();
            if (cursor.p >= cursor.end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }
        }
    }
}
