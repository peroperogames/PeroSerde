/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/10/28
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

/// <summary>
/// UTF-16 read-only cursor used by source-generated serializers, mirroring <see cref="JsonCursorUtf8"/> one to one.
/// The key prefix uses two ulongs (first 4 chars + last 4 chars) to keep the same discriminating power as the UTF-8 version.
/// </summary>
public unsafe struct JsonCursorUtf16(char* start, char* end)
{
    /// <summary>Current read position.</summary>
    public char* p = start;

    /// <summary>End of the readable range (exclusive).</summary>
    public readonly char* end = end;

    /// <summary>Skips one or more whitespace characters.</summary>
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

    /// <summary>Skips whitespace and then tries to consume one character.</summary>
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

    /// <summary>Skips whitespace and then consumes one character, otherwise throws.</summary>
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
    /// Reads one object member name. Returning false means the object has ended (the current character is not a quote).
    /// Outputs the member name length and its first 8 chars (two ulongs) prefix for switch dispatch.
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

        // Escape state machine: cannot rely on "is the previous char a backslash", otherwise a key ending with \\ would swallow the closing quote
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
        p++; // Closing quote

        SkipWhiteSpace();
        if (p >= end || *p != ':')
        {
            throw SerdeException.ThrowInvalidJson("Invalid json format.");
        }

        p++;
        SkipWhiteSpace();
        return true;
    }

    /// <summary>Skips one complete value (used for unknown members).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipValue()
    {
        p += JsonReader.SkipValueUtf16(new ReadOnlySpan<char>(p, (int) (end - p)));
    }
}