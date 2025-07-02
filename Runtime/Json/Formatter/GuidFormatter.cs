/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/05
 */

using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// Guid 的格式化器
    /// </summary>
    [PublicAPI]
    public sealed class GuidFormatter : IFixedSerdeJsonFormatter<Guid>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 38; // 36 + 2引号

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 38;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(Guid value, byte* dest)
        {
            dest[0] = (byte) '\"';
            new GuidBits(ref value).WriteUtf8(dest + 1);
            dest[37] = (byte) '\"';
            return 38;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(Guid value, char* dest)
        {
            dest[0] = '\"';
            new GuidBits(ref value).WriteUtf16(dest + 1);
            dest[37] = '\"';
            return 38;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Guid DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var span = JsonReader.ReadValueUntilEndUtf8(src, 38, out advance);
            if (!Utf8Parser.TryParse(span, out Guid guid, out var consumed) || consumed != span.Length)
                throw SerdeException.ThrowInvalidParsing();
            return guid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Guid DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var span = JsonReader.ReadValueUntilEndUtf16(src, 38, out advance);
            var format = 'D';
            if (!Guid.TryParseExact(span, MemoryMarshal.CreateSpan(ref format, 1), out var value))
                throw SerdeException.ThrowInvalidParsing();
            return value;
        }
    }
}