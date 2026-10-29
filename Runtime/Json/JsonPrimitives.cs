/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Description: 供生成代码直接调用的基础类型读取器（P1 原型）
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 基础类型的直接读取入口。
    /// 生成代码直接调用这些静态方法，跳过 <see cref="SerdeJsonFormatterProvider"/>
    /// 的每值查找与 <c>ISerdeJsonFormatter</c> 的接口分发。
    /// </summary>
    /// <remarks>
    /// 每种类型的解析逻辑只有一份实现：这里全部转发给 <see cref="JsonReader"/> 的
    /// <c>ReadXxxUtf8</c>，内置 formatter 走的是同一批入口，保证两条路径行为一致。
    /// 本类只负责「跳过空白 + 推进游标」。
    /// </remarks>
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
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadIntUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadLongUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadUintUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadUlongUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadShortUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadUshortUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadByteUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadSbyteUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadFloatUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadDoubleUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadDecimalUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadBoolUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadCharUtf8(cursor.p, out var advance);
            cursor.p += advance;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(ref JsonCursor cursor)
        {
            BeginPrimitive(ref cursor);
            var value = JsonReader.ReadStringValueUtf8(cursor.p, (int) (cursor.end - cursor.p), out var advance);
            cursor.p += advance;
            return value;
        }

        /// <summary>跳过空白并确认还有内容可读。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BeginPrimitive(ref JsonCursor cursor)
        {
            cursor.SkipWhiteSpace();
            if (cursor.p >= cursor.end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }
        }
    }
}
