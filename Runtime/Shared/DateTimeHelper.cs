/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/26
 */

using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde
{
    /// <summary>
    /// 时间戳帮助类
    /// </summary>
    internal static class DateTimeHelper
    {
        /// <summary>
        /// 将 DateTime 转换成秒级 Unix 时间戳
        /// </summary>
        public static long DateTimeToUnixTimeSeconds(DateTime dateTime) => ((DateTimeOffset) dateTime.ToUniversalTime()).ToUnixTimeSeconds();

        /// <summary>
        /// 将秒级 Unix 时间戳转换成 DateTime
        /// </summary>
        public static DateTime UnixTimeSecondsToDateTime(long seconds) => DateTimeOffset.FromUnixTimeSeconds(seconds).DateTime.ToLocalTime();

        /// <summary>
        /// 将 DateTime 转换成毫秒级 Unix 时间戳
        /// </summary>
        public static long DateTimeToUnixTimeMilliseconds(DateTime dateTime) => ((DateTimeOffset) dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();

        /// <summary>
        /// 将毫秒级 Unix 时间戳转换成 DateTime
        /// </summary>
        public static DateTime UnixTimeMillisecondsToDateTime(long milliseconds) => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime.ToLocalTime();

        /// <summary>
        /// 将 UTF8 字符串转成 DateTime
        /// </summary>
        public static bool TryParseDateTime(in ReadOnlySpan<byte> span, out DateTime dateTime)
        {
            if (TryParseDefaultDateTime(span, out dateTime) || TryParseChineseDateTime(span, out dateTime))
            {
                return true;
            }
            if (Utf8Parser.TryParse(span, out DateTime value, out var bytesConsumed, 'G') && bytesConsumed == span.Length)
            {
                dateTime = value;
                return true;
            }

            if (Utf8Parser.TryParse(span, out value, out bytesConsumed, 'O') && bytesConsumed == span.Length)
            {
                dateTime = value;
                return true;
            }

            if (Utf8Parser.TryParse(span, out value, out bytesConsumed, 'R') && bytesConsumed == span.Length)
            {
                dateTime = value;
                return true;
            }

            if (Utf8Parser.TryParse(span, out value, out bytesConsumed, 'I') && bytesConsumed == span.Length)
            {
                dateTime = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 以默认格式解析 DateTime。例如 "2024-12-26T10:30:45.123Z"
        /// </summary>
        private static bool TryParseDefaultDateTime(ReadOnlySpan<byte> span, out DateTime dateTime)
        {
            dateTime = default;
            // Parse year, month, day
            if (span.Length < 10 || span[4] != (byte) '-' || span[7] != (byte) '-')
                return false;

            var year = ParseInt(span.Slice(0, 4));
            var month = ParseInt(span.Slice(5, 2));
            var day = ParseInt(span.Slice(8, 2));

            int hour = 0, minute = 0, second = 0, millisecond = 0;

            // Parse time if present
            if (span.Length <= 10 || span[10] != (byte) 'T')
            {
                dateTime = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
                return true;
            }
            if (span.Length < 19)
            {
                return false;
            }

            hour = ParseInt(span.Slice(11, 2));
            minute = ParseInt(span.Slice(14, 2));
            second = ParseInt(span.Slice(17, 2));

            var index = 19;

            // Parse milliseconds if present
            if (index < span.Length && span[index] == (byte) '.')
            {
                index++;
                var msEnd = index;
                while (msEnd < span.Length && span[msEnd] >= (byte) '0' && span[msEnd] <= (byte) '9')
                    msEnd++;

                if (msEnd > index)
                    millisecond = ParseInt(span.Slice(index, Math.Min(3, msEnd - index)));
            }
            dateTime = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
            return true;

            static int ParseInt(ReadOnlySpan<byte> span)
            {
                var result = 0;
                for (var i = 0; i < span.Length; i++)
                {
                    if (span[i] < (byte) '0' || span[i] > (byte) '9')
                        throw SerdeException.ThrowInvalidParsing();
                    result = result * 10 + (span[i] - (byte) '0');
                }
                return result;
            }
        }

        /// <summary>
        /// 以中文格式解析 DateTime。例如 "2024年12月26日 10时30分45秒"
        /// </summary>
        private static bool TryParseChineseDateTime(ReadOnlySpan<byte> span, out DateTime result)
        {
            result = default;
            if (span.Length < 11) // Minimum length for "yyyy年MM月dd日"
                return false;

            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var second = 0;
            var index = 0;
            // Parse year
            while (index < span.Length && span[index] >= (byte) '0' && span[index] <= (byte) '9')
            {
                year = year * 10 + (span[index] - (byte) '0');
                index++;
            }
            if (index + 3 > span.Length || !IsChineseChar(span.Slice(index, 3), "年")) return false;
            index += 3;

            // Parse month
            month = ParseTwoDigits(span, ref index);
            if (index + 3 > span.Length || !IsChineseChar(span.Slice(index, 3), "月")) return false;
            index += 3;

            // Parse day
            day = ParseTwoDigits(span, ref index);
            if (index + 3 > span.Length || !IsChineseChar(span.Slice(index, 3), "日")) return false;
            index += 3;

            // Parse time if present
            if (index < span.Length)
            {
                if (span[index] != (byte) ' ') return false;
                index++;

                hour = ParseTwoDigits(span, ref index);
                if (index + 3 > span.Length || !IsChineseChar(span.Slice(index, 3), "时")) return false;
                index += 3;

                minute = ParseTwoDigits(span, ref index);
                if (index + 3 > span.Length || !IsChineseChar(span.Slice(index, 3), "分")) return false;
                index += 3;

                if (index < span.Length)
                {
                    second = ParseTwoDigits(span, ref index);
                    if (index + 3 > span.Length || !IsChineseChar(span.Slice(index, 3), "秒")) return false;
                }
            }

            result = new DateTime(year, month, day, hour, minute, second);
            return true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool IsChineseChar(ReadOnlySpan<byte> span, string expected) =>
                span.Length == 3 &&
                Encoding.UTF8.GetString(span) == expected;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int ParseTwoDigits(ReadOnlySpan<byte> span, ref int index)
            {
                if (index + 1 >= span.Length) return -1;
                var result = (span[index] - (byte) '0') * 10 + (span[index + 1] - (byte) '0');
                index += 2;
                return result;
            }
        }
    }
}