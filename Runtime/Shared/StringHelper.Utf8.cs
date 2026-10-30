using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde
{
    /// <summary>高性能字符串辅助类</summary>
    /// <remarks>
    /// 能实现高性能主要靠以下几方面:<br/>
    /// 1. 减少循环次数，在每次循环中执行多次操作，从而减少内存寻址次数<br/>
    /// 2. 尽量使用二分查找代替线性查找, 例如计算数字位数时<br/>
    /// 3. 直接使用指针操作<br/>
    /// 4. 能缓存尽量缓存
    /// </remarks>
    [PublicAPI]
    public static unsafe partial class StringHelper
    {
        #region Trim

        /// <summary>
        /// 去除字符串两端的空白字符，然后返回一个新的字符串。
        /// </summary>
        /// <param name="bytes">原始字符串</param>
        /// <param name="length">原始长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> Trim(byte* bytes, int length)
        {
            while (length > 0 && IsWhiteSpace(*bytes))
            {
                ++bytes;
                --length;
            }
            while (length > 0 && IsWhiteSpace(bytes[length - 1]))
            {
                --length;
            }
            return new Span<byte>(bytes, length);
        }

        /// <summary>去除字符串头部的空白字符，然后返回一个新的字符串。</summary>
        /// <param name="bytes">原始字符串</param>
        /// <param name="length">原始长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> TrimStart(byte* bytes, int length)
        {
            while (length > 0 && IsWhiteSpace(*bytes))
            {
                ++bytes;
                --length;
            }
            return new Span<byte>(bytes, length);
        }

        /// <summary>去除字符串尾部的空白字符，然后返回一个新的字符串。</summary>
        /// <param name="bytes">原始字符串</param>
        /// <param name="length">原始长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> TrimEnd(byte* bytes, int length)
        {
            while (length > 0 && IsWhiteSpace(bytes[length - 1]))
            {
                --length;
            }
            return new Span<byte>(bytes, length);
        }

        #endregion

        /// <summary>取指针处前 8 字节（小端、不足补 0）作为 key 前缀，供 switch 分发。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKeyPrefix(byte* start, int length)
        {
            if (length >= 8)
            {
                return Unsafe.ReadUnaligned<ulong>(start);
            }

            ulong value = 0;
            for (var i = 0; i < length; i++)
            {
                value |= (ulong) start[i] << (i * 8);
            }
            return value;
        }
        /// <summary>判断一个字符是否为空白字符</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(byte c) =>
            c == 0x20 || (c >= 0x9 && c <= 0xd) || c == 0x85 || c == 0xa0;

        /// <summary>
        /// 在字符串中找到指定字符的索引，没找到则返回 -1。
        /// </summary>
        /// <param name="bytes">字符串</param>
        /// <param name="c">字符</param>
        /// <param name="length">字符串长度</param>
        /// <returns>返回一个 int 值。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(byte* bytes, int length, byte c)
        {
            // 一次比较 16 字节（4 个 uint）; 命中块内可能有跨字节借位误报, 用标量在块内确认
            if (length < 16)
            {
                return SlowIndexOf(bytes, length, c);
            }

            var broadcast = (uint) c * 0x0101_0101u;
            var offset = bytes;
            var remaining = length;
            while (remaining >= 16)
            {
                var v0 = Unsafe.ReadUnaligned<uint>(offset) ^ broadcast;
                var v1 = Unsafe.ReadUnaligned<uint>(offset + 4) ^ broadcast;
                var v2 = Unsafe.ReadUnaligned<uint>(offset + 8) ^ broadcast;
                var v3 = Unsafe.ReadUnaligned<uint>(offset + 12) ^ broadcast;
                if ((ZeroByteMask(v0) | ZeroByteMask(v1) | ZeroByteMask(v2) | ZeroByteMask(v3)) != 0)
                {
                    var found = SlowIndexOf(offset, 16, c);
                    if (found >= 0)
                    {
                        return (int) (offset - bytes) + found;
                    }
                }

                offset += 16;
                remaining -= 16;
            }

            var tail = SlowIndexOf(offset, remaining, c);
            return tail >= 0 ? (int) (offset - bytes) + tail : -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ZeroByteMask(uint v) => (v - 0x0101_0101u) & ~v & 0x8080_8080u;

        /// <summary>
        /// 在字符串中找到指定两个字符中任意字符的索引，没找到则返回 -1。
        /// </summary>
        /// <param name="bytes">字符串</param>
        /// <param name="length">字符串长度</param>
        /// <param name="c1">字符1</param>
        /// <param name="c2">字符2</param>
        /// <returns>返回一个 int 值。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(byte* bytes, int length, byte c1, byte c2)
        {
            if (length < 16)
            {
                return SlowIndexOfAny(bytes, length, c1, c2);
            }

            var broadcast1 = (uint) c1 * 0x0101_0101u;
            var broadcast2 = (uint) c2 * 0x0101_0101u;
            var offset = bytes;
            var remaining = length;
            while (remaining >= 16)
            {
                var v0 = Unsafe.ReadUnaligned<uint>(offset);
                var v1 = Unsafe.ReadUnaligned<uint>(offset + 4);
                var v2 = Unsafe.ReadUnaligned<uint>(offset + 8);
                var v3 = Unsafe.ReadUnaligned<uint>(offset + 12);

                var m0 = ZeroByteMask(v0 ^ broadcast1) | ZeroByteMask(v0 ^ broadcast2);
                var m1 = ZeroByteMask(v1 ^ broadcast1) | ZeroByteMask(v1 ^ broadcast2);
                var m2 = ZeroByteMask(v2 ^ broadcast1) | ZeroByteMask(v2 ^ broadcast2);
                var m3 = ZeroByteMask(v3 ^ broadcast1) | ZeroByteMask(v3 ^ broadcast2);

                if ((m0 | m1 | m2 | m3) != 0)
                {
                    var found = SlowIndexOfAny(offset, 16, c1, c2);
                    if (found >= 0)
                    {
                        return (int) (offset - bytes) + found;
                    }
                }

                offset += 16;
                remaining -= 16;
            }

            var tail = SlowIndexOfAny(offset, remaining, c1, c2);
            return tail >= 0 ? (int) (offset - bytes) + tail : -1;
        }

        /// <summary>
        /// 检索字符串中是否存在指定字符。
        /// </summary>
        /// <param name="bytes">字符串</param>
        /// <param name="length">字符串长度</param>
        /// <param name="c">指定字符</param>
        /// <returns>返回是否存在</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(byte* bytes, int length, byte c)
        {
            if (length < 16)
            {
                return SlowContains(bytes, length, c);
            }

            var broadcast = (uint) c * 0x0101_0101u;
            var offset = bytes;
            var remaining = length;
            while (remaining >= 16)
            {
                var v0 = Unsafe.ReadUnaligned<uint>(offset) ^ broadcast;
                var v1 = Unsafe.ReadUnaligned<uint>(offset + 4) ^ broadcast;
                var v2 = Unsafe.ReadUnaligned<uint>(offset + 8) ^ broadcast;
                var v3 = Unsafe.ReadUnaligned<uint>(offset + 12) ^ broadcast;
                if ((ZeroByteMask(v0) | ZeroByteMask(v1) | ZeroByteMask(v2) | ZeroByteMask(v3)) != 0 && SlowContains(offset, 16, c))
                {
                    return true;
                }

                offset += 16;
                remaining -= 16;
            }

            return remaining > 0 && SlowContains(offset, remaining, c);
        }

        #region Slow Methods

        private static int SlowIndexOf(byte* bytes, int length, byte c)
        {
            var index = 0;

            if ((length -= 8) >= 0)
            {
                do
                {
                    if (bytes[0] == c) return index + 0;
                    if (bytes[1] == c) return index + 1;
                    if (bytes[2] == c) return index + 2;
                    if (bytes[3] == c) return index + 3;
                    if (bytes[4] == c) return index + 4;
                    if (bytes[5] == c) return index + 5;
                    if (bytes[6] == c) return index + 6;
                    if (bytes[7] == c) return index + 7;

                    bytes += 8;
                    index += 8;
                } while (index <= length);
            }

            if (index - length <= 7 && bytes[0] == c) return index + 0;
            if (index - length <= 6 && bytes[1] == c) return index + 1;
            if (index - length <= 5 && bytes[2] == c) return index + 2;
            if (index - length <= 4 && bytes[3] == c) return index + 3;
            if (index - length <= 3 && bytes[4] == c) return index + 4;
            if (index - length <= 2 && bytes[5] == c) return index + 5;
            if (index - length <= 1 && bytes[6] == c) return index + 6;

            return -1;
        }

        private static int SlowIndexOfAny(byte* bytes, int length, byte c1, byte c2)
        {
            var index = 0;

            if ((length -= 8) >= 0)
            {
                do
                {
                    if (bytes[0] == c1 || bytes[0] == c2) return index + 0;
                    if (bytes[1] == c1 || bytes[1] == c2) return index + 1;
                    if (bytes[2] == c1 || bytes[2] == c2) return index + 2;
                    if (bytes[3] == c1 || bytes[3] == c2) return index + 3;
                    if (bytes[4] == c1 || bytes[4] == c2) return index + 4;
                    if (bytes[5] == c1 || bytes[5] == c2) return index + 5;
                    if (bytes[6] == c1 || bytes[6] == c2) return index + 6;
                    if (bytes[7] == c1 || bytes[7] == c2) return index + 7;

                    bytes += 8;
                    index += 8;
                } while (index <= length);
            }

            if (index - length <= 7 && (bytes[0] == c1 || bytes[0] == c2)) return index + 0;
            if (index - length <= 6 && (bytes[1] == c1 || bytes[1] == c2)) return index + 1;
            if (index - length <= 5 && (bytes[2] == c1 || bytes[2] == c2)) return index + 2;
            if (index - length <= 4 && (bytes[3] == c1 || bytes[3] == c2)) return index + 3;
            if (index - length <= 3 && (bytes[4] == c1 || bytes[4] == c2)) return index + 4;
            if (index - length <= 2 && (bytes[5] == c1 || bytes[5] == c2)) return index + 5;
            if (index - length <= 1 && (bytes[6] == c1 || bytes[6] == c2)) return index + 6;

            return -1;
        }

        private static bool SlowContains(byte* bytes, int length, byte c)
        {
            var index = 0;

            if ((length -= 8) >= 0)
            {
                do
                {
                    if (bytes[0] == c) return true;
                    if (bytes[1] == c) return true;
                    if (bytes[2] == c) return true;
                    if (bytes[3] == c) return true;
                    if (bytes[4] == c) return true;
                    if (bytes[5] == c) return true;
                    if (bytes[6] == c) return true;
                    if (bytes[7] == c) return true;

                    bytes += 8;
                    index += 8;
                } while (index <= length);
            }

            if (index - length <= 7 && bytes[0] == c) return true;
            if (index - length <= 6 && bytes[1] == c) return true;
            if (index - length <= 5 && bytes[2] == c) return true;
            if (index - length <= 4 && bytes[3] == c) return true;
            if (index - length <= 3 && bytes[4] == c) return true;
            if (index - length <= 2 && bytes[5] == c) return true;
            if (index - length <= 1 && bytes[6] == c) return true;

            return false;
        }

        #endregion
    }
}