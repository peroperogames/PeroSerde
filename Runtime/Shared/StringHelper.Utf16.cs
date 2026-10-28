/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/20
 */

using System;
using System.Runtime.CompilerServices;

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
    public static unsafe partial class StringHelper
    {
        private const ulong hash_seed = 0x97121819UL;

        #region Equals

        /// <summary>判断 utf-8 和 utf-16 字符串是否相等，不考虑对齐。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsUnaligned(ReadOnlySpan<byte> utf8, ReadOnlySpan<char> utf16)
        {
            if (utf8.Length != utf16.Length)
            {
                return false;
            }

            for (var i = 0; i < utf8.Length; i++)
            {
                if (utf8[i] != utf16[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>判断两个 utf-16 字符串是否相等。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ReadOnlySpan<char> utf16X, ReadOnlySpan<char> utf16Y)
        {
            if (utf16X.Length != utf16Y.Length) return false;

            fixed (char* fixedPtrX = utf16X)
            fixed (char* fixedPtrY = utf16Y)
            {
                var x = (ulong*) fixedPtrX;
                var y = (ulong*) fixedPtrY;
                var length = utf16X.Length;

                while (length >= 4)
                {
                    if (Unsafe.ReadUnaligned<ulong>(x) != Unsafe.ReadUnaligned<ulong>(y))
                    {
                        return false;
                    }

                    length -= 4;
                    ++x;
                    ++y;
                }
                // 处理剩余的字符
                // 直接对 x 解引用在某些平台上可能会导致访问越界，因此改为逐个比较
                // return ((*x ^ *y) & ~(ulong.MaxValue << (length * 16))) == 0;
                var charX = (char*) x;
                var charY = (char*) y;
                while (length > 0)
                {
                    if (*charX != *charY)
                    {
                        return false;
                    }

                    length -= 1;
                    ++charX;
                    ++charY;
                }
                return true;
            }
        }

        /// <summary>判断两个 utf-8 字符串是否相等。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ReadOnlySpan<byte> utf8X, ReadOnlySpan<byte> utf8Y)
        {
            if (utf8X.Length != utf8Y.Length) return false;

            fixed (byte* fixedPtrX = utf8X)
            fixed (byte* fixedPtrY = utf8Y)
            {
                var x = (ulong*) fixedPtrX;
                var y = (ulong*) fixedPtrY;
                var length = utf8X.Length;

                while (length >= 8)
                {
                    if (Unsafe.ReadUnaligned<ulong>(x) != Unsafe.ReadUnaligned<ulong>(y))
                    {
                        return false;
                    }

                    length -= 8;
                    ++x;
                    ++y;
                }
                // 处理剩余的字符
                // 直接对 x 解引用在某些平台上可能会导致访问越界，因此改为逐个比较
                // return ((*x ^ *y) & ~(ulong.MaxValue << (length * 8))) == 0;
                var byteX = (byte*) x;
                var byteY = (byte*) y;
                while (length > 0)
                {
                    if (*byteX != *byteY)
                    {
                        return false;
                    }

                    length -= 1;
                    ++byteX;
                    ++byteY;
                }
                return true;
            }
        }

        /// <summary>判断两个 utf-16 字符串是否相等。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(string strX, string strY)
        {
            if (strX.Length != strY.Length) return false;

            ref var x = ref Unsafe.As<char, ulong>(ref GetRawStringData(strX));
            ref var y = ref Unsafe.As<char, ulong>(ref GetRawStringData(strY));
            var length = strX.Length;

            while (length >= 4)
            {
                if (Unsafe.ReadUnaligned<ulong>(ref Unsafe.As<ulong, byte>(ref x)) != Unsafe.ReadUnaligned<ulong>(ref Unsafe.As<ulong, byte>(ref y)))
                {
                    return false;
                }

                length -= 4;
                x = ref Unsafe.Add(ref x, 1);
                y = ref Unsafe.Add(ref y, 1);
            }
            // 处理剩余的字符
            // 直接对 x 解引用在某些平台上可能会导致访问越界，因此改为逐个比较
            // return ((x ^ y) & ~(ulong.MaxValue << (length * 16))) == 0;
            ref var charX = ref Unsafe.As<ulong, char>(ref x);
            ref var charY = ref Unsafe.As<ulong, char>(ref y);
            while (length > 0)
            {
                if (charX != charY)
                {
                    return false;
                }

                length -= 1;
                charX = ref Unsafe.Add(ref charX, 1);
                charY = ref Unsafe.Add(ref charY, 1);
            }
            return true;
        }

        #endregion

        #region EqualsWithIgnoreCase

        /// <summary>判断两个 utf-16 字符串是否相等。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsWithIgnoreCase(Span<char> utf16X, Span<char> utf16Y)
        {
            const ulong lower = 0x2020202020202020;

            if (utf16X.Length != utf16Y.Length) return false;

            fixed (char* fixedPtrX = utf16X)
            fixed (char* fixedPtrY = utf16Y)
            {
                var x = (ulong*) fixedPtrX;
                var y = (ulong*) fixedPtrY;
                var length = utf16X.Length;

                while (length >= 4)
                {
                    if ((Unsafe.ReadUnaligned<ulong>(x) | lower) != (Unsafe.ReadUnaligned<ulong>(y) | lower))
                    {
                        return false;
                    }

                    length -= 4;
                    ++x;
                    ++y;
                }
                // 处理剩余的字符
                // 直接对 x 解引用在某些平台上可能会导致访问越界，因此改为逐个比较
                // return (((*x | lower) ^ (*y | lower)) & ~(ulong.MaxValue << (length * 16))) == 0;
                var charX = (char*) x;
                var charY = (char*) y;
                while (length > 0)
                {
                    if ((*charX | 0x20) != (*charY | 0x20))
                    {
                        return false;
                    }

                    length -= 1;
                    ++charX;
                    ++charY;
                }
                return true;
            }
        }

        /// <summary>判断两个 utf-8 字符串是否相等。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsWithIgnoreCase(Span<byte> utf8X, Span<byte> utf8Y)
        {
            const ulong lower = 0x2020202020202020;

            if (utf8X.Length != utf8Y.Length) return false;

            fixed (byte* fixedPtrX = utf8X)
            fixed (byte* fixedPtrY = utf8Y)
            {
                var x = (ulong*) fixedPtrX;
                var y = (ulong*) fixedPtrY;
                var length = utf8X.Length;

                while (length >= 8)
                {
                    if ((Unsafe.ReadUnaligned<ulong>(x) | lower) != (Unsafe.ReadUnaligned<ulong>(y) | lower))
                    {
                        return false;
                    }

                    length -= 8;
                    ++x;
                    ++y;
                }
                // 处理剩余的字符
                // 直接对 x 解引用在某些平台上可能会导致访问越界，因此改为逐个比较
                // return (((*x | lower) ^ (*y | lower)) & (~(ulong.MaxValue << (length * 8)))) == 0;
                var byteX = (byte*) x;
                var byteY = (byte*) y;
                while (length > 0)
                {
                    if ((*byteX | 0x20) != (*byteY | 0x20))
                    {
                        return false;
                    }

                    length -= 1;
                    ++byteX;
                    ++byteY;
                }
                return true;
            }
        }

        /// <summary>判断一个字符串和一个 utf-16 字符串是否相等。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsWithIgnoreCase(Span<char> utf16, string str)
        {
            const ulong lower = 0x2020202020202020;

            if (utf16.Length != str.Length) return false;

            fixed (char* fixedPtrX = utf16)
            {
                var x = (ulong*) fixedPtrX;
                ref var y = ref Unsafe.As<char, ulong>(ref GetRawStringData(str));
                var length = utf16.Length;

                while (length >= 4)
                {
                    if ((Unsafe.ReadUnaligned<ulong>(x) | lower) != (Unsafe.ReadUnaligned<ulong>(ref Unsafe.As<ulong, byte>(ref y)) | lower))
                    {
                        return false;
                    }

                    length -= 4;
                    ++x;
                    y = ref Unsafe.Add(ref y, 1);
                }
                // 处理剩余的字符
                // 直接对 x 解引用在某些平台上可能会导致访问越界，因此改为逐个比较
                // return (((*x | lower) ^ (y | lower)) & ~(ulong.MaxValue << (length * 16))) == 0;
                var charX = (char*) x;
                while (length > 0)
                {
                    if ((*charX | 0x20) != (Unsafe.Add(ref Unsafe.As<ulong, char>(ref y), 0) | 0x20))
                    {
                        return false;
                    }

                    length -= 1;
                    ++charX;
                    y = ref Unsafe.Add(ref y, 1);
                }
                return true;
            }
        }

        #endregion

        #region Trim

        /// <summary>
        /// 去除字符串两端的空白字符，然后返回一个新的字符串。
        /// </summary>
        /// <param name="chars">原始字符串</param>
        /// <param name="length">原始长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<char> Trim(char* chars, int length)
        {
            while (length > 0 && IsWhiteSpace(*chars))
            {
                ++chars;
                --length;
            }
            while (length > 0 && IsWhiteSpace(chars[length - 1]))
            {
                --length;
            }
            return new Span<char>(chars, length);
        }

        /// <summary>去除字符串头部的空白字符，然后返回一个新的字符串。</summary>
        /// <param name="chars">原始字符串</param>
        /// <param name="length">原始长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<char> TrimStart(char* chars, int length)
        {
            while (length > 0 && IsWhiteSpace(*chars))
            {
                ++chars;
                --length;
            }
            return new Span<char>(chars, length);
        }

        /// <summary>去除字符串尾部的空白字符，然后返回一个新的字符串。</summary>
        /// <param name="chars">原始字符串</param>
        /// <param name="length">原始长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<char> TrimEnd(char* chars, int length)
        {
            while (length > 0 && IsWhiteSpace(chars[length - 1]))
            {
                --length;
            }
            return new Span<char>(chars, length);
        }

        #endregion

        /// <summary>将小写英文字符转为大写英文字符。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpper(char c)
            => c >= 'a' && c <= 'z'
                ? (char) (c & ~0x20)
                : c;

        /// <summary>将大写英文字符转为小写英文字符。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToLower(char c)
            => c >= 'A' && c <= 'Z'
                ? (char) (c | 0x20)
                : c;

        /// <summary>将小写英文字符转为大写英文字符。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToUpper(byte c)
            => c >= 'a' && c <= 'z'
                ? (byte) (c & ~0x20)
                : c;

        /// <summary>将大写英文字符转为小写英文字符。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToLower(byte c) =>
            c >= 'A' && c <= 'Z'
                ? (byte) (c | 0x20)
                : c;

        /// <summary>判断一个字符是否为空白字符</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>取 span 前 8 个字符（每 4 个字符打包成一个 ulong）作为 key 前缀，供 switch 分发。</summary>
        public static void GetKeyPrefix(ReadOnlySpan<char> key, out ulong low, out ulong high)
        {
            low = 0;
            high = 0;
            var length = key.Length;
            var first = length < 4 ? length : 4;
            for (var i = 0; i < first; i++)
            {
                low |= (ulong) key[i] << (i * 16);
            }

            var second = length - 4;
            if (second > 4)
            {
                second = 4;
            }

            for (var i = 0; i < second; i++)
            {
                high |= (ulong) key[i + 4] << (i * 16);
            }
        }

        public static bool IsWhiteSpace(char c) =>
            c == 0x20 || (c >= 0x9 && c <= 0xd) || c == 0x85 || c == 0xa0;

        /// <summary>
        /// 在字符串中找到指定字符的索引，没找到则返回 -1。
        /// </summary>
        /// <param name="chars">字符串</param>
        /// <param name="c">字符</param>
        /// <param name="length">字符串长度</param>
        /// <returns>返回一个 int 值。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(char* chars, int length, char c)
        {
            // 一次比较 8 个字符（4 个 uint，每 uint 装 2 个 char）; 误报用标量在块内确认
            if (length < 8)
            {
                return SlowIndexOf(chars, length, c);
            }

            var broadcast = (uint) c * 0x0001_0001u;
            var offset = chars;
            var remaining = length;
            while (remaining >= 8)
            {
                var v0 = Unsafe.ReadUnaligned<uint>(offset) ^ broadcast;
                var v1 = Unsafe.ReadUnaligned<uint>(offset + 2) ^ broadcast;
                var v2 = Unsafe.ReadUnaligned<uint>(offset + 4) ^ broadcast;
                var v3 = Unsafe.ReadUnaligned<uint>(offset + 6) ^ broadcast;
                if ((ZeroCharMask(v0) | ZeroCharMask(v1) | ZeroCharMask(v2) | ZeroCharMask(v3)) != 0)
                {
                    var found = SlowIndexOf(offset, 8, c);
                    if (found >= 0)
                    {
                        return (int) (offset - chars) + found;
                    }
                }

                offset += 8;
                remaining -= 8;
            }

            var tail = SlowIndexOf(offset, remaining, c);
            return tail >= 0 ? (int) (offset - chars) + tail : -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ZeroCharMask(uint v) => (v - 0x0001_0001u) & ~v & 0x8000_8000u;

        /// <summary>
        /// 在字符串中找到指定两个字符中任意字符的索引，没找到则返回 -1。
        /// </summary>
        /// <param name="chars">字符串</param>
        /// <param name="length">字符串长度</param>
        /// <param name="c1">字符1</param>
        /// <param name="c2">字符2</param>
        /// <returns>返回一个 int 值。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(char* chars, int length, char c1, char c2)
        {
            if (length < 8)
            {
                return SlowIndexOfAny(chars, length, c1, c2);
            }

            var broadcast1 = (uint) c1 * 0x0001_0001u;
            var broadcast2 = (uint) c2 * 0x0001_0001u;
            var offset = chars;
            var remaining = length;
            while (remaining >= 8)
            {
                var v0 = Unsafe.ReadUnaligned<uint>(offset);
                var v1 = Unsafe.ReadUnaligned<uint>(offset + 2);
                var v2 = Unsafe.ReadUnaligned<uint>(offset + 4);
                var v3 = Unsafe.ReadUnaligned<uint>(offset + 6);

                var m0 = ZeroCharMask(v0 ^ broadcast1) | ZeroCharMask(v0 ^ broadcast2);
                var m1 = ZeroCharMask(v1 ^ broadcast1) | ZeroCharMask(v1 ^ broadcast2);
                var m2 = ZeroCharMask(v2 ^ broadcast1) | ZeroCharMask(v2 ^ broadcast2);
                var m3 = ZeroCharMask(v3 ^ broadcast1) | ZeroCharMask(v3 ^ broadcast2);

                if ((m0 | m1 | m2 | m3) != 0)
                {
                    var found = SlowIndexOfAny(offset, 8, c1, c2);
                    if (found >= 0)
                    {
                        return (int) (offset - chars) + found;
                    }
                }

                offset += 8;
                remaining -= 8;
            }

            var tail = SlowIndexOfAny(offset, remaining, c1, c2);
            return tail >= 0 ? (int) (offset - chars) + tail : -1;
        }

        /// <summary>
        /// 检索字符串中是否存在指定字符。
        /// </summary>
        /// <param name="chars">字符串</param>
        /// <param name="length">字符串长度</param>
        /// <param name="c">指定字符</param>
        /// <returns>返回是否存在</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(char* chars, int length, char c)
        {
            if (length < 8)
            {
                return SlowContains(chars, length, c);
            }

            var broadcast = (uint) c * 0x0001_0001u;
            var offset = chars;
            var remaining = length;
            while (remaining >= 8)
            {
                var v0 = Unsafe.ReadUnaligned<uint>(offset) ^ broadcast;
                var v1 = Unsafe.ReadUnaligned<uint>(offset + 2) ^ broadcast;
                var v2 = Unsafe.ReadUnaligned<uint>(offset + 4) ^ broadcast;
                var v3 = Unsafe.ReadUnaligned<uint>(offset + 6) ^ broadcast;
                if ((ZeroCharMask(v0) | ZeroCharMask(v1) | ZeroCharMask(v2) | ZeroCharMask(v3)) != 0 && SlowContains(offset, 8, c))
                {
                    return true;
                }

                offset += 8;
                remaining -= 8;
            }

            return remaining > 0 && SlowContains(offset, remaining, c);
        }

        /// <summary>获取字符串的元数据引用。</summary>
        /// <returns>返回第一个字符的引用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref char GetRawStringData(string str)
        {
#if NET7_0_OR_GREATER
            // C#11支持str.GetPinnableReference(), 但由于C#12 ref readonly特性的加入, 需要C#12以上才能将只读引用正常传递给Unsafe.AsRef()
            fixed (char* fixedPtr = str)
            {
                return ref Unsafe.AsRef<char>(fixedPtr);
            }
#else
            return ref Unsafe.AsRef(in str.AsSpan().GetPinnableReference());
#endif
        }

        /// <summary>判断是否含有需要 JSON 转义的字符（&lt; 0x20、双引号或反斜杠），向量化扫描。</summary>
        public static bool ContainsEscapeChar(ReadOnlySpan<char> chars)
        {
            fixed (char* p = chars)
            {
                return ContainsEscapeChar(p, chars.Length);
            }
        }

        /// <summary>判断一段 UTF16 字符序列是否含有需要转义的字符（&lt; 0x20、双引号或反斜杠）。</summary>
        public static bool ContainsEscapeChar(char* chars, int length)
        {
            // 8 个字符一跳（4×uint，每 uint 装 2 个 char）
            var pos = 0;
            var vectorLimit = length - 8;
            if (vectorLimit >= 0)
            {
                var quote = (uint) '"' * 0x0001_0001u;
                var slash = (uint) '\\' * 0x0001_0001u;
                while (pos <= vectorLimit)
                {
                    var p = chars + pos;
                    var v0 = Unsafe.ReadUnaligned<uint>(p);
                    var v1 = Unsafe.ReadUnaligned<uint>(p + 2);
                    var v2 = Unsafe.ReadUnaligned<uint>(p + 4);
                    var v3 = Unsafe.ReadUnaligned<uint>(p + 6);
                    var hit = ControlCharMask(v0) | ControlCharMask(v1) | ControlCharMask(v2) | ControlCharMask(v3)
                              | ZeroCharMask(v0 ^ quote) | ZeroCharMask(v1 ^ quote) | ZeroCharMask(v2 ^ quote) | ZeroCharMask(v3 ^ quote)
                              | ZeroCharMask(v0 ^ slash) | ZeroCharMask(v1 ^ slash) | ZeroCharMask(v2 ^ slash) | ZeroCharMask(v3 ^ slash);
                    if (hit != 0)
                    {
                        // 跨通道借位可能误报，块内用标量确认
                        return SlowContainsEscapeChar(p, 8);
                    }

                    pos += 8;
                }
            }

            return pos < length && SlowContainsEscapeChar(chars + pos, length - pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ControlCharMask(uint v) => (v - 0x0020_0020u) & ~v & 0x8000_8000u;

        private static bool SlowContainsEscapeChar(char* chars, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var c = chars[i];
                if (c < 0x20 || c is '"' or '\\')
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>判断一个UTF16字符串是否所有的字符都为 ASCII 字符。</summary>
        public static bool IsAsciiString(string str)
        {
            var length = str.Length;
            // ASCII 的最大范围: U+0000 ~ U+007F
            const ushort asciiMax = 0x7f;
            // 4 个字符的最大 ASCII 范围表示: 0b1111111110000000 0b1111111110000000 0b1111111110000000 0b1111111110000000
            const ulong ascii4Max = 0xff80ff80ff80ff80;

            ref var first = ref GetRawStringData(str);

            // 一次性判断 4 个字符
            while (length >= 4)
            {
                // & 操作 =0 表明在范围内
                // 如果这四个字符中有任何一个设置了非 ASCII 位，我们就看到了非 ASCII 数据
                if ((Unsafe.ReadUnaligned<ulong>(ref Unsafe.As<char, byte>(ref first)) & ascii4Max) != 0)
                {
                    return false;
                }

                length -= 4;
                first = ref Unsafe.Add(ref first, 4);
            }

            if (length > 0 && Unsafe.Add(ref first, 0) > asciiMax)
                return false;
            if (length > 1 && Unsafe.Add(ref first, 1) > asciiMax)
                return false;
            if (length > 2 && Unsafe.Add(ref first, 2) > asciiMax)
                return false;

            return true;
        }

        /// <summary>判断一个UTF8字符串是否所有的字符都为 ASCII 字符。</summary>
        public static bool IsAsciiString(Span<byte> str)
        {
            var length = str.Length;
            fixed (byte* fixedPtr = str)
            {
                var first = fixedPtr;
                while (length >= 4)
                {
                    if ((Unsafe.ReadUnaligned<uint>(first) & 0x80808080) != 0)
                    {
                        return false;
                    }

                    length -= 4;
                    first += 4;
                }

                if (length > 0 && first[0] > 0x7f)
                    return false;
                if (length > 1 && first[1] > 0x7f)
                    return false;
                if (length > 2 && first[2] > 0x7f)
                    return false;

                return true;
            }
        }

        #region Slow Methods

        private static int SlowIndexOf(char* chars, int length, char c)
        {
            var index = 0;

            if ((length -= 8) >= 0)
            {
                do
                {
                    if (chars[0] == c) return index + 0;
                    if (chars[1] == c) return index + 1;
                    if (chars[2] == c) return index + 2;
                    if (chars[3] == c) return index + 3;
                    if (chars[4] == c) return index + 4;
                    if (chars[5] == c) return index + 5;
                    if (chars[6] == c) return index + 6;
                    if (chars[7] == c) return index + 7;

                    chars += 8;
                    index += 8;
                } while (index <= length);
            }

            if (index - length <= 7 && chars[0] == c) return index + 0;
            if (index - length <= 6 && chars[1] == c) return index + 1;
            if (index - length <= 5 && chars[2] == c) return index + 2;
            if (index - length <= 4 && chars[3] == c) return index + 3;
            if (index - length <= 3 && chars[4] == c) return index + 4;
            if (index - length <= 2 && chars[5] == c) return index + 5;
            if (index - length <= 1 && chars[6] == c) return index + 6;

            return -1;
        }

        private static int SlowIndexOfAny(char* chars, int length, char c1, char c2)
        {
            var index = 0;

            if ((length -= 8) >= 0)
            {
                do
                {
                    if (chars[0] == c1 || chars[0] == c2) return index + 0;
                    if (chars[1] == c1 || chars[1] == c2) return index + 1;
                    if (chars[2] == c1 || chars[2] == c2) return index + 2;
                    if (chars[3] == c1 || chars[3] == c2) return index + 3;
                    if (chars[4] == c1 || chars[4] == c2) return index + 4;
                    if (chars[5] == c1 || chars[5] == c2) return index + 5;
                    if (chars[6] == c1 || chars[6] == c2) return index + 6;
                    if (chars[7] == c1 || chars[7] == c2) return index + 7;

                    chars += 8;
                    index += 8;
                } while (index <= length);
            }

            if (index - length <= 7 && (chars[0] == c1 || chars[0] == c2)) return index + 0;
            if (index - length <= 6 && (chars[1] == c1 || chars[1] == c2)) return index + 1;
            if (index - length <= 5 && (chars[2] == c1 || chars[2] == c2)) return index + 2;
            if (index - length <= 4 && (chars[3] == c1 || chars[3] == c2)) return index + 3;
            if (index - length <= 3 && (chars[4] == c1 || chars[4] == c2)) return index + 4;
            if (index - length <= 2 && (chars[5] == c1 || chars[5] == c2)) return index + 5;
            if (index - length <= 1 && (chars[6] == c1 || chars[6] == c2)) return index + 6;

            return -1;
        }

        private static bool SlowContains(char* chars, int length, char c)
        {
            var index = 0;

            if ((length -= 8) >= 0)
            {
                do
                {
                    if (chars[0] == c) return true;
                    if (chars[1] == c) return true;
                    if (chars[2] == c) return true;
                    if (chars[3] == c) return true;
                    if (chars[4] == c) return true;
                    if (chars[5] == c) return true;
                    if (chars[6] == c) return true;
                    if (chars[7] == c) return true;

                    chars += 8;
                    index += 8;
                } while (index <= length);
            }

            if (index - length <= 7 && chars[0] == c) return true;
            if (index - length <= 6 && chars[1] == c) return true;
            if (index - length <= 5 && chars[2] == c) return true;
            if (index - length <= 4 && chars[3] == c) return true;
            if (index - length <= 3 && chars[4] == c) return true;
            if (index - length <= 2 && chars[5] == c) return true;
            if (index - length <= 1 && chars[6] == c) return true;

            return false;
        }

        #endregion
    }
}