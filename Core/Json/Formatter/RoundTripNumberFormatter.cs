/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/10/10
 */

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

/// <summary>
/// Shared implementation of round-trip floating-point writing.
/// </summary>
internal static class RoundTripNumberHelper
{
    // Maximum character count of the "R" format.
    internal const int max_float_size = 16;
    internal const int max_double_size = 25;

    internal static unsafe int Format(float value, char* dest)
    {
        if (NumberHelper.IsSpecialValue(value))
        {
            throw SerdeException.ThrowInvalidFloatPoint();
        }
        // Write negative zero explicitly: do not rely on the runtime "R" formatting (the obvious value < 0 sign check misses -0.0)
        if (Unsafe.As<float, uint>(ref value) == 0x80000000U)
        {
            dest[0] = '-';
            dest[1] = '0';
            return 2;
        }
        if (!value.TryFormat(new Span<char>(dest, max_float_size), out var written, "R", CultureInfo.InvariantCulture))
        {
            throw SerdeException.ThrowInvalidFloatPoint();
        }
        return written;
    }

    internal static unsafe int Format(double value, char* dest)
    {
        if (NumberHelper.IsSpecialValue(value))
        {
            throw SerdeException.ThrowInvalidFloatPoint();
        }
        // Write negative zero explicitly: do not rely on the runtime "R" formatting (the obvious value < 0 sign check misses -0.0)
        if (Unsafe.As<double, ulong>(ref value) == 0x8000000000000000UL)
        {
            dest[0] = '-';
            dest[1] = '0';
            return 2;
        }
        if (!value.TryFormat(new Span<char>(dest, max_double_size), out var written, "R", CultureInfo.InvariantCulture))
        {
            throw SerdeException.ThrowInvalidFloatPoint();
        }
        return written;
    }
}

/// <summary>Round-trip formatter for float</summary>
public sealed class RoundTripFloatFormatter : IFixedSerdeJsonFormatter<float>
{
    public const int max_size = RoundTripNumberHelper.max_float_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf8() => max_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf16() => max_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf8Unsafe(float value, byte* dest) => WriteUtf8(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf16Unsafe(float value, char* dest) => RoundTripNumberHelper.Format(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe float DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadFloatUtf8(src, out advance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe float DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadFloatUtf16(src, out advance);

    // "R" output is ASCII-only, so each char can be truncated to a byte
    internal static unsafe int WriteUtf8(float value, byte* dest)
    {
        var buffer = stackalloc char[max_size];
        var written = RoundTripNumberHelper.Format(value, buffer);
        for (var i = 0; i < written; i++)
        {
            dest[i] = (byte) buffer[i];
        }
        return written;
    }
}

/// <summary>Round-trip formatter for double</summary>
public sealed class RoundTripDoubleFormatter : IFixedSerdeJsonFormatter<double>
{
    public const int max_size = RoundTripNumberHelper.max_double_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf8() => max_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf16() => max_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf8Unsafe(double value, byte* dest) => WriteUtf8(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf16Unsafe(double value, char* dest) => RoundTripNumberHelper.Format(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe double DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadDoubleUtf8(src, out advance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe double DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadDoubleUtf16(src, out advance);

    internal static unsafe int WriteUtf8(double value, byte* dest)
    {
        var buffer = stackalloc char[max_size];
        var written = RoundTripNumberHelper.Format(value, buffer);
        for (var i = 0; i < written; i++)
        {
            dest[i] = (byte) buffer[i];
        }
        return written;
    }
}

/// <summary>Round-trip formatter for float?</summary>
public sealed class NullableRoundTripFloatFormatter : IFixedSerdeJsonFormatter<float?>
{
    public const int max_size = RoundTripNumberHelper.max_float_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf8() => max_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf16() => max_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf8Unsafe(float? value, byte* dest) =>
        value == null ? JsonWriter.WriteNullUtf8(dest) : RoundTripFloatFormatter.WriteUtf8(value.Value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf16Unsafe(float? value, char* dest) =>
        value == null ? JsonWriter.WriteNullUtf16(dest) : RoundTripNumberHelper.Format(value.Value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe float? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableFloatUtf8(src, out advance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe float? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableFloatUtf16(src, out advance);
}

/// <summary>Round-trip formatter for double?</summary>
public sealed class NullableRoundTripDoubleFormatter : IFixedSerdeJsonFormatter<double?>
{
    public const int max_size = RoundTripNumberHelper.max_double_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf8() => max_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf16() => max_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf8Unsafe(double? value, byte* dest) =>
        value == null ? JsonWriter.WriteNullUtf8(dest) : RoundTripDoubleFormatter.WriteUtf8(value.Value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf16Unsafe(double? value, char* dest) =>
        value == null ? JsonWriter.WriteNullUtf16(dest) : RoundTripNumberHelper.Format(value.Value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe double? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableDoubleUtf8(src, out advance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe double? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableDoubleUtf16(src, out advance);
}