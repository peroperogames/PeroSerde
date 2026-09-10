using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

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
        public static int IndexOf(byte* bytes, int length, byte c) =>
            Vector.IsHardwareAccelerated
                ? FastIndexOf(bytes, length, c)
                : SlowIndexOf(bytes, length, c);

        /// <summary>
        /// 在字符串中找到指定两个字符中任意字符的索引，没找到则返回 -1。
        /// </summary>
        /// <param name="bytes">字符串</param>
        /// <param name="length">字符串长度</param>
        /// <param name="c1">字符1</param>
        /// <param name="c2">字符2</param>
        /// <returns>返回一个 int 值。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(byte* bytes, int length, byte c1, byte c2) =>
            Vector.IsHardwareAccelerated
                ? FastIndexOfAny(bytes, length, c1, c2)
                : SlowIndexOfAny(bytes, length, c1, c2);

        /// <summary>
        /// 检索字符串中是否存在指定字符。
        /// </summary>
        /// <param name="bytes">字符串</param>
        /// <param name="length">字符串长度</param>
        /// <param name="c">指定字符</param>
        /// <returns>返回是否存在</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(byte* bytes, int length, byte c) =>
            Vector.IsHardwareAccelerated
                ? FastContains(bytes, length, c)
                : SlowContains(bytes, length, c);

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

        #region SIMD Accelerated Methods

        private static int FastIndexOf(byte* bytes, int length, byte c)
        {
            var offset = bytes;

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
                    var matches = Vector.Equals(Unsafe.ReadUnaligned<Vector<ushort>>(offset), comparison);

                    if (matches != Vector<ushort>.Zero)
                    {
                        return (int) (offset - bytes) + LocateFirstFoundChar(matches);
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
            return (int) (offset - bytes);
        }

        private static int FastIndexOfAny(byte* bytes, int length, byte c1, byte c2)
        {
            var offset = bytes;

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
                        Vector.Equals(Unsafe.ReadUnaligned<Vector<ushort>>(offset), comparison1),
                        Vector.Equals(Unsafe.ReadUnaligned<Vector<ushort>>(offset), comparison2)
                    );

                    if (matches != Vector<ushort>.Zero)
                    {
                        return (int) (offset - bytes) + LocateFirstFoundChar(matches);
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
            return (int) (offset - bytes);
        }

        private static bool FastContains(byte* bytes, int length, byte c)
        {
            var offset = bytes;

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
                    var matches = Vector.Equals(Unsafe.ReadUnaligned<Vector<ushort>>(offset), comparison);

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