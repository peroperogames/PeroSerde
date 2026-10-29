/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/10/28
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

/// <summary>
/// Read-only cursor used by source-generated serializers.
/// Unlike <see cref="JsonReaderUtf8"/>, the cursor is held by the caller as a local variable,
/// so each field scan is straight-line work on a local pointer and does not keep writing reader state back.
/// </summary>
public unsafe struct JsonCursorUtf8(byte* start, byte* end)
{
    /// <summary>Current read position.</summary>
    public byte* p = start;

    /// <summary>End of the readable range (exclusive).</summary>
    public readonly byte* end = end;

    /// <summary>Skips one or more whitespace characters.</summary>
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

    /// <summary>Skips whitespace and then tries to consume one character.</summary>
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

    /// <summary>Skips whitespace and then consumes one character, otherwise throws.</summary>
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
    /// Reads one object member name. Returning false means the object has ended (the current character is not a quote).
    /// Outputs the member name length and its first 8 bytes (little-endian) prefix for switch dispatch.
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

        // Escape state machine: cannot rely on "is the previous byte a backslash", otherwise a key ending with \\ would swallow the closing quote
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
        p++; // Closing quote

        SkipWhiteSpace();
        if (p >= end || *p != (byte) ':')
        {
            throw SerdeException.ThrowInvalidJson("Invalid json format.");
        }

        p++;
        SkipWhiteSpace();
        return true;
    }

    /// <summary>Skips one complete value (used for unknown members).</summary>
    public void SkipValue()
    {
        p += JsonReader.SkipValueUtf8(new ReadOnlySpan<byte>(p, (int) (end - p)));
    }
}