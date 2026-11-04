/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/20
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde;

/// <summary>High-performance string helper</summary>
/// <remarks>
/// High performance is achieved mainly through the following:<br/>
/// 1. Reduce loop iterations by performing multiple operations per iteration, thus reducing memory addressing<br/>
/// 2. Prefer binary search over linear search where possible, for example when counting digits<br/>
/// 3. Use direct pointer operations<br/>
/// 4. Cache as much as possible
/// </remarks>
public static unsafe partial class StringHelper
{
    #region Equals

    /// <summary>Determines whether a utf-8 string and a utf-16 string are equal, ignoring alignment.</summary>
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

    /// <summary>Determines whether two utf-16 strings are equal.</summary>
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
            // Handle the remaining characters
            // Dereferencing x directly may cause an out-of-bounds access on some platforms, so compare character by character
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

    /// <summary>Determines whether two utf-8 strings are equal.</summary>
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
            // Handle the remaining characters
            // Dereferencing x directly may cause an out-of-bounds access on some platforms, so compare character by character
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

    /// <summary>Determines whether two utf-16 strings are equal.</summary>
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
        // Handle the remaining characters
        // Dereferencing x directly may cause an out-of-bounds access on some platforms, so compare character by character
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char ToLowerAscii(char c) => c >= 'A' && c <= 'Z' ? (char) (c | 0x20) : c;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ToLowerAscii(byte c) => c >= (byte) 'A' && c <= (byte) 'Z' ? (byte) (c | 0x20) : c;

    #region EqualsWithIgnoreCase

    // Only ASCII letters are folded: UTF-8 bytes and UTF-16 characters then agree on every input, and characters that
    // are not case variants of each other (such as '[' / '{') never compare equal. Note that a per-character OR with
    // 0x20 (or 0x2020 for a pair of lanes) is not a lower-casing operation and produces false positives.

    /// <summary>Determines whether two utf-16 strings are equal, ignoring ASCII letter case.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsWithIgnoreCase(Span<char> utf16X, Span<char> utf16Y)
    {
        if (utf16X.Length != utf16Y.Length) return false;

        for (var i = 0; i < utf16X.Length; i++)
        {
            if (ToLowerAscii(utf16X[i]) != ToLowerAscii(utf16Y[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Determines whether two utf-8 strings are equal, ignoring ASCII letter case.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsWithIgnoreCase(Span<byte> utf8X, Span<byte> utf8Y)
    {
        if (utf8X.Length != utf8Y.Length) return false;

        for (var i = 0; i < utf8X.Length; i++)
        {
            if (ToLowerAscii(utf8X[i]) != ToLowerAscii(utf8Y[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Determines whether a string and a utf-16 string are equal, ignoring ASCII letter case.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsWithIgnoreCase(Span<char> utf16, string str)
    {
        if (utf16.Length != str.Length) return false;

        for (var i = 0; i < utf16.Length; i++)
        {
            if (ToLowerAscii(utf16[i]) != ToLowerAscii(str[i]))
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Trim

    /// <summary>
    /// Removes whitespace characters from both ends of the string and returns a new string.
    /// </summary>
    /// <param name="chars">Original string</param>
    /// <param name="length">Original length</param>
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

    /// <summary>Removes whitespace characters from the start of the string and returns a new string.</summary>
    /// <param name="chars">Original string</param>
    /// <param name="length">Original length</param>
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

    /// <summary>Removes whitespace characters from the end of the string and returns a new string.</summary>
    /// <param name="chars">Original string</param>
    /// <param name="length">Original length</param>
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

    /// <summary>Converts a lowercase English character to uppercase.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToUpper(char c)
        => c >= 'a' && c <= 'z'
            ? (char) (c & ~0x20)
            : c;

    /// <summary>Converts an uppercase English character to lowercase.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToLower(char c)
        => c >= 'A' && c <= 'Z'
            ? (char) (c | 0x20)
            : c;

    /// <summary>Converts a lowercase English character to uppercase.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ToUpper(byte c)
        => c >= 'a' && c <= 'z'
            ? (byte) (c & ~0x20)
            : c;

    /// <summary>Converts an uppercase English character to lowercase.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ToLower(byte c) =>
        c >= 'A' && c <= 'Z'
            ? (byte) (c | 0x20)
            : c;

    /// <summary>Takes the first 8 characters of the span (4 characters packed into each ulong) as a key prefix for switch dispatch.</summary>
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

    /// <summary>Determines whether a character is a whitespace character</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(char c) =>
        c == 0x20 || (c >= 0x9 && c <= 0xd) || c == 0x85 || c == 0xa0;

    /// <summary>
    /// Finds the index of the specified character in the string, or returns -1 if not found.
    /// </summary>
    /// <param name="chars">String</param>
    /// <param name="c">Character</param>
    /// <param name="length">String length</param>
    /// <returns>Returns an int value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(char* chars, int length, char c)
    {
        // Compares 8 characters (4 uints, each uint holds 2 chars) at a time; confirm false positives inside the block with scalars
        if (length < 8)
        {
            return SlowIndexOf(chars, length, c);
        }

        var broadcast = c * 0x0001_0001u;
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
    /// Finds the index of either of the two specified characters in the string, or returns -1 if not found.
    /// </summary>
    /// <param name="chars">String</param>
    /// <param name="length">String length</param>
    /// <param name="c1">Character 1</param>
    /// <param name="c2">Character 2</param>
    /// <returns>Returns an int value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfAny(char* chars, int length, char c1, char c2)
    {
        if (length < 8)
        {
            return SlowIndexOfAny(chars, length, c1, c2);
        }

        var broadcast1 = c1 * 0x0001_0001u;
        var broadcast2 = c2 * 0x0001_0001u;
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
    /// Checks whether the specified character exists in the string.
    /// </summary>
    /// <param name="chars">String</param>
    /// <param name="length">String length</param>
    /// <param name="c">Specified character</param>
    /// <returns>Returns whether it exists</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(char* chars, int length, char c)
    {
        if (length < 8)
        {
            return SlowContains(chars, length, c);
        }

        var broadcast = c * 0x0001_0001u;
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

    /// <summary>Gets the raw data reference of the string.</summary>
    /// <returns>Returns a reference to the first character</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetRawStringData(string str)
    {
#if NET7_0_OR_GREATER
            // C# 11 supports str.GetPinnableReference(), but with the C# 12 ref readonly feature, C# 12 or later is required to pass a read-only reference to Unsafe.AsRef() correctly
            fixed (char* fixedPtr = str)
            {
                return ref Unsafe.AsRef<char>(fixedPtr);
            }
#else
        return ref Unsafe.AsRef(in str.AsSpan().GetPinnableReference());
#endif
    }

    /// <summary>Determines whether the chars contain characters that need JSON escaping (&lt; 0x20, double quote or backslash), using a vectorized scan.</summary>
    public static bool ContainsEscapeChar(ReadOnlySpan<char> chars)
    {
        fixed (char* p = chars)
        {
            return ContainsEscapeChar(p, chars.Length);
        }
    }

    /// <summary>Determines whether a UTF16 character sequence contains characters that need escaping (&lt; 0x20, double quote or backslash).</summary>
    public static bool ContainsEscapeChar(char* chars, int length)
    {
        // Stride of 8 characters (4 uints, each uint holds 2 chars)
        var pos = 0;
        var vectorLimit = length - 8;
        if (vectorLimit >= 0)
        {
            var quote = '"' * 0x0001_0001u;
            var slash = '\\' * 0x0001_0001u;
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
                    // Cross-lane borrows may cause false positives; confirm inside the block with scalars
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
    /// <summary>Determines whether all characters of a UTF16 string are ASCII characters.</summary>
    public static bool IsAsciiString(string str)
    {
        var length = str.Length;
        // Maximum ASCII range: U+0000 ~ U+007F
        const ushort asciiMax = 0x7f;
        // Bit mask of the maximum ASCII range for 4 characters: 0b1111111110000000 0b1111111110000000 0b1111111110000000 0b1111111110000000
        const ulong ascii4Max = 0xff80ff80ff80ff80;

        ref var first = ref GetRawStringData(str);

        // Check 4 characters at a time
        while (length >= 4)
        {
            // An AND result of 0 means the value is in range
            // If any of these four characters has a non-ASCII bit set, non-ASCII data is present
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

    /// <summary>Determines whether all characters of a UTF8 string are ASCII characters.</summary>
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