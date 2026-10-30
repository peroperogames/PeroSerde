/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/10/28
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 供源生成的序列化器使用的 UTF-16 只读游标，与 <see cref="JsonCursorUtf8"/> 一一对应。
    /// key 前缀用两个 ulong 表示（前 4 个字符 + 后 4 个字符），保证与 UTF-8 版同等的区分度。
    /// </summary>
    public unsafe struct JsonCursorUtf16
    {
        /// <summary>当前读取位置。</summary>
        public char* p;

        /// <summary>可读区间的结束位置（不含）。</summary>
        public readonly char* end;

        public JsonCursorUtf16(char* start, char* end)
        {
            p = start;
            this.end = end;
        }

        /// <summary>跳过一个或多个空白字符。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipWhiteSpace()
        {
            while (p < end)
            {
                if (StringHelper.IsWhiteSpace(*p))
                {
                    p++;
                    continue;
                }

                break;
            }
        }

        /// <summary>跳过空白后尝试消费一个字符。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConsume(char c)
        {
            SkipWhiteSpace();
            if (p < end && *p == c)
            {
                p++;
                return true;
            }

            return false;
        }

        /// <summary>跳过空白后消费一个字符，否则抛错。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Expect(char c)
        {
            SkipWhiteSpace();
            if (p >= end || *p != c)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            p++;
        }

        /// <summary>
        /// 读取一个对象成员名。返回 false 表示已到对象结束（当前字符不是引号）。
        /// 输出成员名长度与其前 8 个字符（两个 ulong）前缀，供 switch 分发。
        /// </summary>
        public bool TryReadKey(out int keyLength, out ulong prefixLow, out ulong prefixHigh)
        {
            SkipWhiteSpace();
            keyLength = 0;
            prefixLow = 0;
            prefixHigh = 0;

            if (p >= end || *p != '"')
            {
                return false;
            }

            p++;
            var start = p;

            // 转义状态机: 不能用「看前一个字符是不是反斜杠」, 否则 key 以 \\ 结尾时会吞掉闭引号
            var escaped = false;
            while (p < end)
            {
                var c = *p;
                if (escaped)
                {
                    escaped = false;
                    p++;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    p++;
                    continue;
                }

                if (c == '"')
                {
                    break;
                }

                p++;
            }

            if (p >= end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            keyLength = (int) (p - start);
            StringHelper.GetKeyPrefix(new ReadOnlySpan<char>(start, keyLength), out prefixLow, out prefixHigh);
            p++; // 闭引号

            SkipWhiteSpace();
            if (p >= end || *p != ':')
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            p++;
            SkipWhiteSpace();
            return true;
        }

        /// <summary>跳过一个完整的值（未知成员时使用）。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipValue()
        {
            p += JsonReader.SkipValueUtf16(new ReadOnlySpan<char>(p, (int) (end - p)));
        }
    }
}
