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
    #region Trim

    /// <summary>
    /// Removes whitespace characters from both ends of the string and returns a new string.
    /// </summary>
    /// <param name="bytes">Original string</param>
    /// <param name="length">Original length</param>
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

    /// <summary>Removes whitespace characters from the start of the string and returns a new string.</summary>
    /// <param name="bytes">Original string</param>
    /// <param name="length">Original length</param>
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

    /// <summary>Removes whitespace characters from the end of the string and returns a new string.</summary>
    /// <param name="bytes">Original string</param>
    /// <param name="length">Original length</param>
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

    /// <summary>Packs the first eight bytes of the key into a <c>ulong</c> for switch dispatch.</summary>
    /// <remarks>
    /// A key of eight bytes or longer is read as one little endian word; a shorter key is packed the same way but zero
    /// padded, so a caller can compare against a constant that was built from an ASCII member name with the same rule.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetKeyPrefix(ReadOnlySpan<byte> key)
    {
        if (key.Length >= 8)
        {
            return Unsafe.ReadUnaligned<ulong>(ref System.Runtime.InteropServices.MemoryMarshal.GetReference(key));
        }

        ulong value = 0;
        for (var i = 0; i < key.Length; i++)
        {
            value |= (ulong) key[i] << (i * 8);
        }

        return value;
    }

    /// <summary>Takes the first 8 bytes at the pointer (little endian, zero padded) as a key prefix for switch dispatch.</summary>
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

    /// <summary>Compares a UTF8 byte span against a UTF16 string, decoding the bytes as it goes.</summary>
    /// <remarks>
    /// <see cref="StringHelper.EqualsUnaligned(ReadOnlySpan{byte}, ReadOnlySpan{char})"/> assumes one byte per character,
    /// which only holds for ASCII member names. Member names with other characters need the real decoder, and decoding
    /// into a temporary string would allocate on every key comparison.
    /// </remarks>
    public static bool EqualsUtf8(ReadOnlySpan<byte> utf8, ReadOnlySpan<char> utf16)
    {
        var byteIndex = 0;
        var charIndex = 0;
        while (byteIndex < utf8.Length && charIndex < utf16.Length)
        {
            var b = utf8[byteIndex];
            int codePoint;
            if (b < 0x80)
            {
                codePoint = b;
                byteIndex++;
            }
            else if ((b & 0xE0) == 0xC0 && byteIndex + 1 < utf8.Length)
            {
                codePoint = ((b & 0x1F) << 6) | (utf8[byteIndex + 1] & 0x3F);
                byteIndex += 2;
            }
            else if ((b & 0xF0) == 0xE0 && byteIndex + 2 < utf8.Length)
            {
                codePoint = ((b & 0x0F) << 12) | ((utf8[byteIndex + 1] & 0x3F) << 6) | (utf8[byteIndex + 2] & 0x3F);
                byteIndex += 3;
            }
            else if ((b & 0xF8) == 0xF0 && byteIndex + 3 < utf8.Length)
            {
                codePoint = ((b & 0x07) << 18) | ((utf8[byteIndex + 1] & 0x3F) << 12)
                                               | ((utf8[byteIndex + 2] & 0x3F) << 6) | (utf8[byteIndex + 3] & 0x3F);
                byteIndex += 4;
            }
            else
            {
                return false;
            }

            if (codePoint > 0xFFFF)
            {
                // Surrogate pair in the UTF16 side
                if (charIndex + 1 >= utf16.Length)
                {
                    return false;
                }

                codePoint -= 0x10000;
                if (utf16[charIndex] != (char) (0xD800 + (codePoint >> 10))
                    || utf16[charIndex + 1] != (char) (0xDC00 + (codePoint & 0x3FF)))
                {
                    return false;
                }

                charIndex += 2;
                continue;
            }

            if (utf16[charIndex] != (char) codePoint)
            {
                return false;
            }

            charIndex++;
        }

        return byteIndex == utf8.Length && charIndex == utf16.Length;
    }

    /// <summary>Determines whether a character is a whitespace character</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(byte c) =>
        c == 0x20 || (c >= 0x9 && c <= 0xd) || c == 0x85 || c == 0xa0;

    /// <summary>
    /// Finds the index of the specified character in the string, or returns -1 if not found.
    /// </summary>
    /// <param name="bytes">String</param>
    /// <param name="c">Character</param>
    /// <param name="length">String length</param>
    /// <returns>Returns an int value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(byte* bytes, int length, byte c)
    {
        // Compares 16 bytes (4 uints) at a time; a hit may be a cross-byte borrow false positive, so confirm inside the block with scalars
        if (length < 16)
        {
            return SlowIndexOf(bytes, length, c);
        }

        var broadcast = c * 0x0101_0101u;
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
    /// Finds the index of either of the two specified characters in the string, or returns -1 if not found.
    /// </summary>
    /// <param name="bytes">String</param>
    /// <param name="length">String length</param>
    /// <param name="c1">Character 1</param>
    /// <param name="c2">Character 2</param>
    /// <returns>Returns an int value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfAny(byte* bytes, int length, byte c1, byte c2)
    {
        if (length < 16)
        {
            return SlowIndexOfAny(bytes, length, c1, c2);
        }

        var broadcast1 = c1 * 0x0101_0101u;
        var broadcast2 = c2 * 0x0101_0101u;
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
    /// Checks whether the specified character exists in the string.
    /// </summary>
    /// <param name="bytes">String</param>
    /// <param name="length">String length</param>
    /// <param name="c">Specified character</param>
    /// <returns>Returns whether it exists</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(byte* bytes, int length, byte c)
    {
        if (length < 16)
        {
            return SlowContains(bytes, length, c);
        }

        var broadcast = c * 0x0101_0101u;
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