/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Description: 生成代码使用的 UTF-8 JSON 游标（P1 原型）
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 供源生成的序列化器使用的只读游标。
    /// 与 <see cref="JsonReaderUtf8"/> 的区别：游标由调用方持有为局部变量，
    /// 每字段扫描都是对局部指针的直线操作，不需要反复回写 reader 状态。
    /// </summary>
    public unsafe struct JsonCursor
    {
        /// <summary>当前读取位置。</summary>
        public byte* p;

        /// <summary>可读区间的结束位置（不含）。</summary>
        public readonly byte* end;

        public JsonCursor(byte* start, byte* end)
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
                var c = *p;
                if (c == 0x20 || (c >= 0x09 && c <= 0x0d) || c == 0x85 || c == 0xa0)
                {
                    p++;
                    continue;
                }

                break;
            }
        }

        /// <summary>跳过空白后尝试消费一个字符。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConsume(byte c)
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
        public void Expect(byte c)
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
        /// 输出成员名长度与其前 8 字节（小端）前缀，供 switch 分发。
        /// </summary>
        public bool TryReadKey(out int keyLength, out ulong prefix)
        {
            SkipWhiteSpace();
            keyLength = 0;
            prefix = 0;

            if (p >= end || *p != (byte) '"')
            {
                return false;
            }

            p++;
            var start = p;

            // 转义状态机: 不能用「看前一个字节是不是反斜杠」, 否则 key 以 \\ 结尾时会吞掉闭引号
            var escaped = false;
            while (p < end)
            {
                var b = *p;
                if (escaped)
                {
                    escaped = false;
                    p++;
                    continue;
                }

                if (b == (byte) '\\')
                {
                    escaped = true;
                    p++;
                    continue;
                }

                if (b == (byte) '"')
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
            prefix = StringHelper.GetKeyPrefix(start, keyLength);
            p++; // 闭引号

            SkipWhiteSpace();
            if (p >= end || *p != (byte) ':')
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            p++;
            SkipWhiteSpace();
            return true;
        }

        /// <summary>跳过一个完整的值（未知成员时使用）。</summary>
        public void SkipValue()
        {
            JsonReader.ReadValueUntilEndUtf8(p, (int) (end - p), out var advance);
            p += advance;
        }
    }
}
