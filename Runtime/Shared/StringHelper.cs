using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde
{
    /// <summary>高性能字符串辅助类</summary>
    /// <remarks>
    /// 能实现高性能主要靠以下几方面:<br/>
    /// 1. 减少循环次数，在每次循环中执行多次操作，从而减少内存寻址次数<br/>
    /// 2. 如果硬件支持，使用SIMD并行化加速，也就是<see cref="System.Numerics.Vector{T}"/>类
    /// 3. 尽量使用二分查找代替线性查找, 例如计算数字位数时
    /// 4. 直接使用指针操作
    /// 5. 能缓存尽量缓存
    /// </remarks>
    public static unsafe class StringHelper
    {
        private const ulong hash_seed = 0x97121819UL;

        #region Equals

        /// <summary>判断两个 utf-16 字符串是否相等。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(Span<char> utf16X, Span<char> utf16Y)
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
                    if (*x != *y)
                    {
                        return false;
                    }

                    length -= 4;
                    ++x;
                    ++y;
                }

                return ((*x ^ *y) & ~(ulong.MaxValue << (length * 16))) == 0;
            }
        }

        /// <summary>判断两个 utf-8 字符串是否相等。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(Span<byte> utf8X, Span<byte> utf8Y)
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
                    if (*x != *y)
                    {
                        return false;
                    }

                    length -= 8;
                    ++x;
                    ++y;
                }

                return ((*x ^ *y) & ~(ulong.MaxValue << (length * 8))) == 0;
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
                if (x != y)
                {
                    return false;
                }

                length -= 4;
                x = ref Unsafe.Add(ref x, 1);
                y = ref Unsafe.Add(ref y, 1);
            }

            return ((x ^ y) & ~(ulong.MaxValue << (length * 16))) == 0;
        }

        #endregion

        #region GetHashCode

        /// <summary>获取一个 utf-16 字符串的 Hash 值。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(Span<char> utf16)
        {
            fixed (char* fixedPtr = utf16)
            {
                var ptr = (ulong*) fixedPtr;
                var length = utf16.Length;

                var hash = hash_seed;

                while (length >= 4)
                {
                    hash ^= *ptr * hash;

                    length -= 4;
                    ++ptr;
                }

                hash ^= (*ptr & ~(ulong.MaxValue << (length * 16))) * hash;

                return (int) (hash >> 32) ^ (int) hash;
            }
        }

        /// <summary>获取一个 utf-8 字符串的 Hash 值。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(Span<byte> utf8)
        {
            fixed (byte* fixedPtr = utf8)
            {
                var ptr = (ulong*) fixedPtr;
                var length = utf8.Length;

                var hash = hash_seed;

                while (length >= 8)
                {
                    hash ^= *ptr * hash;

                    length -= 8;
                    ++ptr;
                }

                hash ^= (*ptr & ~(ulong.MaxValue << (length * 8))) * hash;

                return (int) (hash >> 32) ^ (int) hash;
            }
        }

        /// <summary>获取字符串 Hash 值。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(string str)
        {
            ref var ptr = ref Unsafe.As<char, ulong>(ref GetRawStringData(str));
            var length = str.Length;

            var hash = hash_seed;

            while (length >= 4)
            {
                hash ^= ptr * hash;

                length -= 4;
                ptr = ref Unsafe.Add(ref ptr, 1);
            }

            hash ^= (ptr & ~(ulong.MaxValue << (length * 16))) * hash;

            return (int) (hash >> 32) ^ (int) hash;
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
                    if ((*x | lower) != (*y | lower))
                    {
                        return false;
                    }

                    length -= 4;
                    ++x;
                    ++y;
                }

                return (((*x | lower) ^ (*y | lower)) & ~(ulong.MaxValue << (length * 16))) == 0;
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
                    if ((*x | lower) != (*y | lower))
                    {
                        return false;
                    }

                    length -= 8;
                    ++x;
                    ++y;
                }

                return (((*x | lower) ^ (*y | lower)) & (~(ulong.MaxValue << (length * 8)))) == 0;
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
                    if ((*x | lower) != (y | lower))
                    {
                        return false;
                    }

                    length -= 4;
                    ++x;
                    y = ref Unsafe.Add(ref y, 1);
                }

                return (((*x | lower) ^ (y | lower)) & ~(ulong.MaxValue << (length * 16))) == 0;
            }
        }

        #endregion

        #region GetHashCodeWithIgnoreCase

        /// <summary>获取一个 utf-16 字符串的 Hash 值。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCodeWithIgnoreCase(Span<char> utf16)
        {
            const ulong lower = 0x2020202020202020;
            fixed (char* fixedPtr = utf16)
            {
                var ptr = (ulong*) fixedPtr;
                var length = utf16.Length;

                var hash = hash_seed;

                while (length >= 4)
                {
                    hash ^= (*ptr | lower) * hash;

                    length -= 4;
                    ++ptr;
                }

                hash ^= ((*ptr & ~(ulong.MaxValue << (length * 16))) | lower) * hash;

                return (int) (hash >> 32) ^ (int) hash;
            }
        }

        /// <summary>获取一个 utf-8 字符串的 Hash 值。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCodeWithIgnoreCase(Span<byte> utf8)
        {
            const ulong lower = 0x2020202020202020;
            fixed (byte* fixedPtr = utf8)
            {
                var ptr = (ulong*) fixedPtr;
                var length = utf8.Length;
                var hash = hash_seed;

                while (length >= 8)
                {
                    hash ^= (*ptr | lower) * hash;

                    length -= 8;
                    ++ptr;
                }

                hash ^= ((*ptr & ~(ulong.MaxValue << (length * 8))) | lower) * hash;
                return (int) (hash >> 32) ^ (int) hash;
            }
        }

        /// <summary>获取字符串 Hash 值。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCodeWithIgnoreCase(string str)
        {
            const ulong lower = 0x2020202020202020;

            ref var ptr = ref Unsafe.As<char, ulong>(ref GetRawStringData(str));
            var length = str.Length;

            var hash = hash_seed;

            while (length >= 4)
            {
                hash ^= (ptr | lower) * hash;

                length -= 4;
                ptr = ref Unsafe.Add(ref ptr, 1);
            }

            hash ^= ((ptr & ~(ulong.MaxValue << (length * 16))) | lower) * hash;

            return (int) (hash >> 32) ^ (int) hash;
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
        public static int IndexOf(char* chars, int length, char c) =>
            Vector.IsHardwareAccelerated
                ? FastIndexOf(chars, length, c)
                : SlowIndexOf(chars, length, c);

        /// <summary>
        /// 在字符串中找到指定两个字符中任意字符的索引，没找到则返回 -1。
        /// </summary>
        /// <param name="chars">字符串</param>
        /// <param name="length">字符串长度</param>
        /// <param name="c1">字符1</param>
        /// <param name="c2">字符2</param>
        /// <returns>返回一个 int 值。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(char* chars, int length, char c1, char c2) =>
            Vector.IsHardwareAccelerated
                ? FastIndexOfAny(chars, length, c1, c2)
                : SlowIndexOfAny(chars, length, c1, c2);

        /// <summary>
        /// 检索字符串中是否存在指定字符。
        /// </summary>
        /// <param name="chars">字符串</param>
        /// <param name="length">字符串长度</param>
        /// <param name="c">指定字符</param>
        /// <returns>返回是否存在</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(char* chars, int length, char c) =>
            Vector.IsHardwareAccelerated
                ? FastContains(chars, length, c)
                : SlowContains(chars, length, c);

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

        /// <summary>判断一个UTF16字符串是否所有的字符都为 ASCII 字符。</summary>
        public static bool IsAsciiString(string str)
        {
            var length = str.Length;
            // ASCII 的最大范围: U+0000 ~ U+007F
            const ushort asciiMax = 0x7f;
            // 4 个字符的最大 ASCII 范围表示: 0b1111111110000000 0b1111111110000000 0b1111111110000000 0b1111111110000000
            const ulong ascii4Max = 0xff80ff80ff80ff80;

            ref var first = ref GetRawStringData(str);
            // 使用 SIMD 加速（如果可以的话），且仅在当前字符串长度大于等于 SIMD 支持的位宽时使用，否则会 out of range
            if (Vector.IsHardwareAccelerated && length >= Vector<ushort>.Count)
            {
                var comparison = new Vector<ushort>(asciiMax);

                do
                {
                    // 单个字符检查是否超出范围
                    if (Vector.GreaterThan(Unsafe.As<char, Vector<ushort>>(ref first), comparison) != Vector<ushort>.Zero)
                    {
                        return false;
                    }

                    length -= Vector<ushort>.Count;
                    first = ref Unsafe.Add(ref first, Vector<ushort>.Count);
                } while (length >= Vector<ushort>.Count);
            }

            // 如果 SIMD 未满，则处理剩下的字符。如果不使用 SIMD，则从头判断
            // 一次性判断 4 个字符
            while (length >= 4)
            {
                // & 操作 =0 表明在范围内
                // 如果这四个字符中有任何一个设置了非 ASCII 位，我们就看到了非 ASCII 数据
                if ((Unsafe.As<char, ulong>(ref first) & ascii4Max) != 0)
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
                    if ((*(uint*) first & 0x80808080) != 0)
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

        #region SIMD Accelerated Methods

        private const ulong xor_power_of_two_to_high_char = (0x03ul | 0x02ul << 16 | 0x01ul << 32) + 1;

        private static int LocateFirstFoundChar(Vector<ushort> matches)
        {
            var match = Unsafe.As<Vector<ushort>, Vector<ulong>>(ref matches)[0];
            if (match != 0) return 0 + LocateFirstFoundChar(match);

            match = Unsafe.As<Vector<ushort>, Vector<ulong>>(ref matches)[1];
            if (match != 0) return 4 + LocateFirstFoundChar(match);

            match = Unsafe.As<Vector<ushort>, Vector<ulong>>(ref matches)[2];
            if (match != 0) return 8 + LocateFirstFoundChar(match);

            match = Unsafe.As<Vector<ushort>, Vector<ulong>>(ref matches)[3];
            return 12 + LocateFirstFoundChar(match);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LocateFirstFoundChar(ulong match)
            => unchecked((int) (((match ^ (match - 1)) * xor_power_of_two_to_high_char) >> 49));

        private static int FastIndexOf(char* chars, int length, char c)
        {
            var offset = chars;

            if (length >= 8)
            {
                if (offset[0] == c) return 0;
                if (offset[1] == c) return 1;
                if (offset[2] == c) return 2;
                if (offset[3] == c) return 3;
                if (offset[4] == c) return 4;
                if (offset[5] == c) return 5;
                if (offset[6] == c) return 6;
                if (offset[7] == c) return 7;

                offset += 8;
                length -= 8;
            }

            if (length >= Vector<ushort>.Count)
            {
                var comparison = new Vector<ushort>(c);

                do
                {
                    var matches = Vector.Equals(*(Vector<ushort>*) offset, comparison);

                    if (matches != Vector<ushort>.Zero)
                    {
                        return (int) (offset - chars) + LocateFirstFoundChar(matches);
                    }

                    length -= Vector<ushort>.Count;
                    offset += Vector<ushort>.Count;
                } while (length >= Vector<ushort>.Count);
            }

            while (length >= 2)
            {
                if (offset[0] == c) goto Found0;
                if (offset[1] == c) goto Found1;

                offset += 2;
                length -= 2;
            }

            if (length >= 0 && offset[0] == c) goto Found0;

            return -1;

            Found1:
            ++offset;
            Found0:
            return (int) (offset - chars);
        }

        private static int FastIndexOfAny(char* chars, int length, char c1, char c2)
        {
            var offset = chars;

            if (length >= 8)
            {
                if (offset[0] == c1 || offset[0] == c2) return 0;
                if (offset[1] == c1 || offset[1] == c2) return 1;
                if (offset[2] == c1 || offset[2] == c2) return 2;
                if (offset[3] == c1 || offset[3] == c2) return 3;
                if (offset[4] == c1 || offset[4] == c2) return 4;
                if (offset[5] == c1 || offset[5] == c2) return 5;
                if (offset[6] == c1 || offset[6] == c2) return 6;
                if (offset[7] == c1 || offset[7] == c2) return 7;

                offset += 8;
                length -= 8;
            }

            if (length >= Vector<ushort>.Count)
            {
                var comparison1 = new Vector<ushort>(c1);
                var comparison2 = new Vector<ushort>(c2);

                do
                {
                    var matches = Vector.BitwiseOr(
                        Vector.Equals(*(Vector<ushort>*) offset, comparison1),
                        Vector.Equals(*(Vector<ushort>*) offset, comparison2)
                    );

                    if (matches != Vector<ushort>.Zero)
                    {
                        return (int) (offset - chars) + LocateFirstFoundChar(matches);
                    }

                    length -= Vector<ushort>.Count;
                    offset += Vector<ushort>.Count;
                } while (length >= Vector<ushort>.Count);
            }

            while (length >= 2)
            {
                if (offset[0] == c1 || offset[0] == c2) goto Found0;
                if (offset[1] == c1 || offset[1] == c2) goto Found1;

                offset += 2;
                length -= 2;
            }

            if (length >= 0 && (offset[0] == c1 || offset[0] == c2)) goto Found0;

            return -1;

            Found1:
            ++offset;
            Found0:
            return (int) (offset - chars);
        }

        private static bool FastContains(char* chars, int length, char c)
        {
            var offset = chars;

            if (length >= 8)
            {
                if (offset[0] == c) return true;
                if (offset[1] == c) return true;
                if (offset[2] == c) return true;
                if (offset[3] == c) return true;
                if (offset[4] == c) return true;
                if (offset[5] == c) return true;
                if (offset[6] == c) return true;
                if (offset[7] == c) return true;

                offset += 8;
                length -= 8;
            }

            if (length >= Vector<ushort>.Count)
            {
                var comparison = new Vector<ushort>(c);

                do
                {
                    var matches = Vector.Equals(*(Vector<ushort>*) offset, comparison);

                    if (matches != Vector<ushort>.Zero)
                    {
                        return true;
                    }

                    length -= Vector<ushort>.Count;
                    offset += Vector<ushort>.Count;
                } while (length >= Vector<ushort>.Count);
            }

            while (length >= 2)
            {
                if (offset[0] == c) return true;
                if (offset[1] == c) return true;

                offset += 2;
                length -= 2;
            }

            if (length >= 0 && offset[0] == c) return true;

            return false;
        }

        #endregion
    }
}