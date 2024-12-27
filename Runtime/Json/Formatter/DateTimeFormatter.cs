/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    // TODO: 按位序列化，不创建 string
    public sealed class DateTimeFormatter : ISerdeJsonFormatter<DateTime>
    {
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(DateTime value, ref JsonWriterUtf8 writer) => writer.WriteString(value.ToString(CultureInfo.CurrentCulture));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(DateTime value, ref JsonWriterUtf16 writer) => writer.WriteString(value.ToString(CultureInfo.CurrentCulture));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime DeserializeUtf8(ref ReadOnlySpan<byte> span, ref JsonReaderUtf8 reader) => SpanConverter.ToDateTime(reader.ReadUntilEnd());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime DeserializeUtf16(ref ReadOnlySpan<char> span, ref JsonReaderUtf16 reader) => SpanConverter.ToDateTime(reader.ReadUntilEnd());
    }

    /// <summary>
    /// 秒级 Unix 时间戳 Json 格式化器
    /// </summary>
    public sealed unsafe class UnixSecondsFormatter : IFixedSerdeJsonFormatter<DateTime>
    {
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SerializeUtf8Unsafe(DateTime value, byte* dest)
        {
            var thick = DateTimeToLong(value);
            dest[0] = NumberHelper.GetAscii((int) (thick / 1000000000 % 10));
            dest[1] = NumberHelper.GetAscii((int) (thick / 100000000 % 10));
            dest[2] = NumberHelper.GetAscii((int) (thick / 10000000 % 10));
            dest[3] = NumberHelper.GetAscii((int) (thick / 1000000 % 10));
            dest[4] = NumberHelper.GetAscii((int) (thick / 100000 % 10));
            dest[5] = NumberHelper.GetAscii((int) (thick / 10000 % 10));
            dest[6] = NumberHelper.GetAscii((int) (thick / 1000 % 10));
            dest[7] = NumberHelper.GetAscii((int) (thick / 100 % 10));
            dest[8] = NumberHelper.GetAscii((int) (thick / 10 % 10));
            dest[9] = NumberHelper.GetAscii((int) (thick % 10));
            return 10;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SerializeUtf16Unsafe(DateTime value, char* dest)
        {
            var thick = DateTimeToLong(value);
            dest[0] = NumberHelper.GetAsciiChar((int) (thick / 1000000000 % 10));
            dest[1] = NumberHelper.GetAsciiChar((int) (thick / 100000000 % 10));
            dest[2] = NumberHelper.GetAsciiChar((int) (thick / 10000000 % 10));
            dest[3] = NumberHelper.GetAsciiChar((int) (thick / 1000000 % 10));
            dest[4] = NumberHelper.GetAsciiChar((int) (thick / 100000 % 10));
            dest[5] = NumberHelper.GetAsciiChar((int) (thick / 10000 % 10));
            dest[6] = NumberHelper.GetAsciiChar((int) (thick / 1000 % 10));
            dest[7] = NumberHelper.GetAsciiChar((int) (thick / 100 % 10));
            dest[8] = NumberHelper.GetAsciiChar((int) (thick / 10 % 10));
            dest[9] = NumberHelper.GetAsciiChar((int) (thick % 10));
            return 10;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            long thick = 0;
            for (var i = 0; i < 9; i++)
            {
                thick += NumberHelper.FromAscii(src[i]);
                thick *= 10;
            }
            thick += NumberHelper.FromAscii(src[9]);
            advance = 10;
            return LongToDateTime(thick);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime DeserializeUtf16Unsafe(char* src, out int advance)
        {
            long thick = 0;
            for (var i = 0; i < 9; i++)
            {
                thick += NumberHelper.FromAscii(src[i]);
                thick *= 10;
            }
            thick += NumberHelper.FromAscii(src[9]);
            advance = 10;
            return LongToDateTime(thick);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long DateTimeToLong(DateTime dateTime) => DateTimeHelper.DateTimeToUnixTimeSeconds(dateTime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime LongToDateTime(long timestamp) => DateTimeHelper.UnixTimeSecondsToDateTime(timestamp);
    }

    /// <summary>
    /// 毫秒级 Unix 时间戳格式化器
    /// </summary>
    public sealed unsafe class UnixMillisecondFormatter : IFixedSerdeJsonFormatter<DateTime>
    {
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 13;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 13;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SerializeUtf8Unsafe(DateTime value, byte* dest)
        {
            var thick = DateTimeToLong(value);
            dest[0] = NumberHelper.GetAscii((int) (thick / 1000000000000 % 10));
            dest[1] = NumberHelper.GetAscii((int) (thick / 100000000000 % 10));
            dest[2] = NumberHelper.GetAscii((int) (thick / 10000000000 % 10));
            dest[3] = NumberHelper.GetAscii((int) (thick / 1000000000 % 10));
            dest[4] = NumberHelper.GetAscii((int) (thick / 100000000 % 10));
            dest[5] = NumberHelper.GetAscii((int) (thick / 10000000 % 10));
            dest[6] = NumberHelper.GetAscii((int) (thick / 1000000 % 10));
            dest[7] = NumberHelper.GetAscii((int) (thick / 100000 % 10));
            dest[8] = NumberHelper.GetAscii((int) (thick / 10000 % 10));
            dest[9] = NumberHelper.GetAscii((int) (thick / 1000 % 10));
            dest[10] = NumberHelper.GetAscii((int) (thick / 100 % 10));
            dest[11] = NumberHelper.GetAscii((int) (thick / 10 % 10));
            dest[12] = NumberHelper.GetAscii((int) (thick % 10));
            return 13;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SerializeUtf16Unsafe(DateTime value, char* dest)
        {
            var thick = DateTimeToLong(value);
            dest[0] = NumberHelper.GetAsciiChar((int) (thick / 1000000000000 % 10));
            dest[1] = NumberHelper.GetAsciiChar((int) (thick / 100000000000 % 10));
            dest[2] = NumberHelper.GetAsciiChar((int) (thick / 10000000000 % 10));
            dest[3] = NumberHelper.GetAsciiChar((int) (thick / 1000000000 % 10));
            dest[4] = NumberHelper.GetAsciiChar((int) (thick / 100000000 % 10));
            dest[5] = NumberHelper.GetAsciiChar((int) (thick / 10000000 % 10));
            dest[6] = NumberHelper.GetAsciiChar((int) (thick / 1000000 % 10));
            dest[7] = NumberHelper.GetAsciiChar((int) (thick / 100000 % 10));
            dest[8] = NumberHelper.GetAsciiChar((int) (thick / 10000 % 10));
            dest[9] = NumberHelper.GetAsciiChar((int) (thick / 1000 % 10));
            dest[10] = NumberHelper.GetAsciiChar((int) (thick / 100 % 10));
            dest[11] = NumberHelper.GetAsciiChar((int) (thick / 10 % 10));
            dest[12] = NumberHelper.GetAsciiChar((int) (thick % 10));
            return 13;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            long thick = 0;
            for (var i = 0; i < 12; i++)
            {
                thick += NumberHelper.FromAscii(src[i]);
                thick *= 10;
            }
            thick += NumberHelper.FromAscii(src[12]);
            advance = 13;
            return LongToDateTime(thick);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime DeserializeUtf16Unsafe(char* src, out int advance)
        {
            long thick = 0;
            for (var i = 0; i < 12; i++)
            {
                thick += NumberHelper.FromAscii(src[i]);
                thick *= 10;
            }
            thick += NumberHelper.FromAscii(src[12]);
            advance = 13;
            return LongToDateTime(thick);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long DateTimeToLong(DateTime dateTime) => DateTimeHelper.DateTimeToUnixTimeMilliseconds(dateTime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime LongToDateTime(long timestamp) => DateTimeHelper.UnixTimeMillisecondsToDateTime(timestamp);
    }
}