using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde;
static partial class NumberHelper {
    /// <summary>Formats a float value as a utf16 string.</summary>
    /// <remarks>
    /// dest must have enough space; at most <see cref="max_float_string_length"/> bytes are required
    /// </remarks>
    internal static unsafe int ToUtf16StringUnsafe(float value, char* dest)
    {
        // TODO How to handle special values? Several modes need to be provided
        if (IsSpecialValue(value))
        {
            throw SerdeException.ThrowInvalidFloatPoint();
        }
        var start = dest;
        // Write the negative sign when needed
        if (value < 0)
        {
            *dest = negative_sign;
            dest++;
            value = -value;
        }
        // Get the exponent part of the floating point number
        var exponent = GetExponent(value);
        // Length of the integer part. If the number of integer digits exceeds the maximum or is negative, scientific notation is used and the integer length is 1
        var lengthOfInteger = exponent >= 0 && exponent < s_MaxFloatDigitalLength ? exponent + 1 : 1;
        // Length of the fractional part + 1 (for rounding)
        var lengthOfFractional = s_MaxFloatDigitalLength - lengthOfInteger;
        // exponent now stands for the exponent in scientific notation. 0 means scientific notation is not needed.
        if (exponent < s_MaxFloatDigitalLength && exponent > -max_floating_zero_length)
        {
            exponent = 0;
        }
        // Treat the significant part of the floating point value as an integer: 123.45678 => 12345678
        var mantissa = (ulong) Pow(value, lengthOfFractional - exponent);

        {
            ulong temp;
            // Round the last digit
            if (lengthOfFractional >= 1)
            {
                temp = mantissa / 10;
                if (mantissa - temp * 10 >= 5)
                    mantissa = temp + 1;
                else
                    mantissa = temp;
                lengthOfFractional -= 1;
            }
            // Strip trailing zeros
            while (lengthOfFractional >= 4 && mantissa == (temp = mantissa / 10000) * 10000)
            {
                mantissa = temp;
                lengthOfFractional -= 4;
            }
            while (lengthOfFractional > 0 && mantissa == (temp = mantissa / 10) * 10)
            {
                mantissa = temp;
                lengthOfFractional -= 1;
            }
        }
        // Write the digits
        if (lengthOfFractional == 0) // No fractional digits: write it as an integer directly
        {
            var charsWritten = ToUtf16StringUnsafe(mantissa, dest);
            if (charsWritten > lengthOfInteger && exponent != 0)
            {
                exponent += 1;
                charsWritten -= 1;
            }
            dest += charsWritten;
        }
        // If the integer part is 0, write the integer part first.
        // exponent == 0 must be required: in scientific notation (0 < value < 1e-4) the first digit of mantissa is the integer part,
        // taking this branch makes firstNumIndex negative and rolling dest back overwrites the decimal point just written,
        // producing illegal JSON numbers like "04940656458412466E-324". That case must go to the branch below (insert the decimal point after the first digit).
        else if (value < 1 && exponent == 0)
        {
            *dest = '0';
            *(dest + 1) = dot_sign;
            dest += 2;
            // The integer is zero, so the decimal point does not need to move; write the fractional part directly
            var fractionalExponent = GetExponent(mantissa);
            var firstNumIndex = lengthOfFractional - (fractionalExponent + 1);
            for (int i = 0; i < firstNumIndex; i++)
            {
                dest[i] = '0';
            }
            dest += firstNumIndex;
            ToUtf16StringUnsafe(mantissa, dest);
            dest += fractionalExponent + 1;
        }
        else
        {
            // With fractional digits, write as an integer first, then write the decimal point
            ToUtf16StringUnsafe(mantissa, dest);
            dest += lengthOfInteger;
            // Shift the whole fractional part one position to make room for the decimal point
            for (var i = lengthOfFractional; i > 0;)
            {
                dest[i] = dest[--i];
            }
            *dest = dot_sign;
            dest += lengthOfFractional + 1;
        }
        // If scientific notation is needed, write the exponent part
        if (exponent != 0)
        {
            dest[0] = exponent_sign;
            if (exponent > 0) {
                dest[1] = positive_sign;
                dest += 2;
                dest += ToUtf16StringUnsafe((ulong) exponent, dest);
            } else {
                dest[1] = negative_sign;
                dest += 2;
                dest += ToUtf16StringUnsafe((ulong) (-exponent), dest);
            }
        }
        return (int) (dest - start);
    }
    /// <summary>Formats a float value as a utf8 string.</summary>
    /// <remarks>
    /// dest must have enough space; at most <see cref="max_float_string_length"/> bytes are required
    /// </remarks>
    internal static unsafe int ToUtf8StringUnsafe(float value, byte* dest)
    {
        // TODO How to handle special values? Several modes need to be provided
        if (IsSpecialValue(value))
        {
            throw SerdeException.ThrowInvalidFloatPoint();
        }
        var start = dest;
        // Write the negative sign when needed
        if (value < 0)
        {
            *dest = (byte) negative_sign;
            dest++;
            value = -value;
        }
        // Get the exponent part of the floating point number
        var exponent = GetExponent(value);
        // Length of the integer part. If the number of integer digits exceeds the maximum or is negative, scientific notation is used and the integer length is 1
        var lengthOfInteger = exponent >= 0 && exponent < s_MaxFloatDigitalLength ? exponent + 1 : 1;
        // Length of the fractional part + 1 (for rounding)
        var lengthOfFractional = s_MaxFloatDigitalLength - lengthOfInteger;
        // exponent now stands for the exponent in scientific notation. 0 means scientific notation is not needed.
        if (exponent < s_MaxFloatDigitalLength && exponent > -max_floating_zero_length)
        {
            exponent = 0;
        }
        // Treat the significant part of the floating point value as an integer: 123.45678 => 12345678
        var mantissa = (ulong) Pow(value, lengthOfFractional - exponent);

        {
            ulong temp;
            // Round the last digit
            if (lengthOfFractional >= 1)
            {
                temp = mantissa / 10;
                if (mantissa - temp * 10 >= 5)
                    mantissa = temp + 1;
                else
                    mantissa = temp;
                lengthOfFractional -= 1;
            }
            // Strip trailing zeros
            while (lengthOfFractional >= 4 && mantissa == (temp = mantissa / 10000) * 10000)
            {
                mantissa = temp;
                lengthOfFractional -= 4;
            }
            while (lengthOfFractional > 0 && mantissa == (temp = mantissa / 10) * 10)
            {
                mantissa = temp;
                lengthOfFractional -= 1;
            }
        }
        // Write the digits
        if (lengthOfFractional == 0) // No fractional digits: write it as an integer directly
        {
            var bytesWritten = ToUtf8StringUnsafe(mantissa, dest);
            if (bytesWritten > lengthOfInteger && exponent != 0)
            {
                exponent += 1;
                bytesWritten -= 1;
            }
            dest += bytesWritten;
        }
        // If the integer part is 0, write the integer part first.
        // exponent == 0 must be required: in scientific notation (0 < value < 1e-4) the first digit of mantissa is the integer part,
        // taking this branch makes firstNumIndex negative and rolling dest back overwrites the decimal point just written,
        // producing illegal JSON numbers like "04940656458412466E-324". That case must go to the branch below (insert the decimal point after the first digit).
        else if (value < 1 && exponent == 0)
        {
            *dest = (byte) '0';
            *(dest + 1) = (byte) dot_sign;
            dest += 2;
            // The integer is zero, so the decimal point does not need to move; write the fractional part directly
            var fractionalExponent = GetExponent(mantissa);
            var firstNumIndex = lengthOfFractional - (fractionalExponent + 1);
            for (int i = 0; i < firstNumIndex; i++)
            {
                dest[i] = (byte) '0';
            }
            dest += firstNumIndex;
            ToUtf8StringUnsafe(mantissa, dest);
            dest += fractionalExponent + 1;
        }
        else
        {
            // With fractional digits, write as an integer first, then write the decimal point
            ToUtf8StringUnsafe(mantissa, dest);
            dest += lengthOfInteger;
            // Shift the whole fractional part one position to make room for the decimal point
            for (var i = lengthOfFractional; i > 0;)
            {
                dest[i] = dest[--i];
            }
            *dest = (byte) dot_sign;
            dest += lengthOfFractional + 1;
        }
        // If scientific notation is needed, write the exponent part
        if (exponent != 0)
        {
            dest[0] = (byte) exponent_sign;
            if (exponent > 0) {
                dest[1] = (byte) positive_sign;
                dest += 2;
                dest += ToUtf8StringUnsafe((ulong) exponent, dest);
            } else {
                dest[1] = (byte) negative_sign;
                dest += 2;
                dest += ToUtf8StringUnsafe((ulong) (-exponent), dest);
            }
        }
        return (int) (dest - start);
    }
    /// <summary>Formats a double value as a utf16 string.</summary>
    /// <remarks>
    /// dest must have enough space; at most <see cref="max_double_string_length"/> bytes are required
    /// </remarks>
    internal static unsafe int ToUtf16StringUnsafe(double value, char* dest)
    {
        // TODO How to handle special values? Several modes need to be provided
        if (IsSpecialValue(value))
        {
            throw SerdeException.ThrowInvalidFloatPoint();
        }
        var start = dest;
        // Write the negative sign when needed
        if (value < 0)
        {
            *dest = negative_sign;
            dest++;
            value = -value;
        }
        // Get the exponent part of the floating point number
        var exponent = GetExponent(value);
        // Length of the integer part. If the number of integer digits exceeds the maximum or is negative, scientific notation is used and the integer length is 1
        var lengthOfInteger = exponent >= 0 && exponent < s_MaxDoubleDigitalLength ? exponent + 1 : 1;
        // Length of the fractional part + 1 (for rounding)
        var lengthOfFractional = s_MaxDoubleDigitalLength - lengthOfInteger;
        // exponent now stands for the exponent in scientific notation. 0 means scientific notation is not needed.
        if (exponent < s_MaxDoubleDigitalLength && exponent > -max_floating_zero_length)
        {
            exponent = 0;
        }
        // Treat the significant part of the floating point value as an integer: 123.45678 => 12345678
        var mantissa = (ulong) Pow(value, lengthOfFractional - exponent);

        {
            ulong temp;
            // Round the last digit
            if (lengthOfFractional >= 1)
            {
                temp = mantissa / 10;
                if (mantissa - temp * 10 >= 5)
                    mantissa = temp + 1;
                else
                    mantissa = temp;
                lengthOfFractional -= 1;
            }
            // Strip trailing zeros
            while (lengthOfFractional >= 4 && mantissa == (temp = mantissa / 10000) * 10000)
            {
                mantissa = temp;
                lengthOfFractional -= 4;
            }
            while (lengthOfFractional > 0 && mantissa == (temp = mantissa / 10) * 10)
            {
                mantissa = temp;
                lengthOfFractional -= 1;
            }
        }
        // Write the digits
        if (lengthOfFractional == 0) // No fractional digits: write it as an integer directly
        {
            var charsWritten = ToUtf16StringUnsafe(mantissa, dest);
            if (charsWritten > lengthOfInteger && exponent != 0)
            {
                exponent += 1;
                charsWritten -= 1;
            }
            dest += charsWritten;
        }
        // If the integer part is 0, write the integer part first.
        // exponent == 0 must be required: in scientific notation (0 < value < 1e-4) the first digit of mantissa is the integer part,
        // taking this branch makes firstNumIndex negative and rolling dest back overwrites the decimal point just written,
        // producing illegal JSON numbers like "04940656458412466E-324". That case must go to the branch below (insert the decimal point after the first digit).
        else if (value < 1 && exponent == 0)
        {
            *dest = '0';
            *(dest + 1) = dot_sign;
            dest += 2;
            // The integer is zero, so the decimal point does not need to move; write the fractional part directly
            var fractionalExponent = GetExponent(mantissa);
            var firstNumIndex = lengthOfFractional - (fractionalExponent + 1);
            for (int i = 0; i < firstNumIndex; i++)
            {
                dest[i] = '0';
            }
            dest += firstNumIndex;
            ToUtf16StringUnsafe(mantissa, dest);
            dest += fractionalExponent + 1;
        }
        else
        {
            // With fractional digits, write as an integer first, then write the decimal point
            ToUtf16StringUnsafe(mantissa, dest);
            dest += lengthOfInteger;
            // Shift the whole fractional part one position to make room for the decimal point
            for (var i = lengthOfFractional; i > 0;)
            {
                dest[i] = dest[--i];
            }
            *dest = dot_sign;
            dest += lengthOfFractional + 1;
        }
        // If scientific notation is needed, write the exponent part
        if (exponent != 0)
        {
            dest[0] = exponent_sign;
            if (exponent > 0) {
                dest[1] = positive_sign;
                dest += 2;
                dest += ToUtf16StringUnsafe((ulong) exponent, dest);
            } else {
                dest[1] = negative_sign;
                dest += 2;
                dest += ToUtf16StringUnsafe((ulong) (-exponent), dest);
            }
        }
        return (int) (dest - start);
    }
    /// <summary>Formats a double value as a utf8 string.</summary>
    /// <remarks>
    /// dest must have enough space; at most <see cref="max_double_string_length"/> bytes are required
    /// </remarks>
    internal static unsafe int ToUtf8StringUnsafe(double value, byte* dest)
    {
        // TODO How to handle special values? Several modes need to be provided
        if (IsSpecialValue(value))
        {
            throw SerdeException.ThrowInvalidFloatPoint();
        }
        var start = dest;
        // Write the negative sign when needed
        if (value < 0)
        {
            *dest = (byte) negative_sign;
            dest++;
            value = -value;
        }
        // Get the exponent part of the floating point number
        var exponent = GetExponent(value);
        // Length of the integer part. If the number of integer digits exceeds the maximum or is negative, scientific notation is used and the integer length is 1
        var lengthOfInteger = exponent >= 0 && exponent < s_MaxDoubleDigitalLength ? exponent + 1 : 1;
        // Length of the fractional part + 1 (for rounding)
        var lengthOfFractional = s_MaxDoubleDigitalLength - lengthOfInteger;
        // exponent now stands for the exponent in scientific notation. 0 means scientific notation is not needed.
        if (exponent < s_MaxDoubleDigitalLength && exponent > -max_floating_zero_length)
        {
            exponent = 0;
        }
        // Treat the significant part of the floating point value as an integer: 123.45678 => 12345678
        var mantissa = (ulong) Pow(value, lengthOfFractional - exponent);

        {
            ulong temp;
            // Round the last digit
            if (lengthOfFractional >= 1)
            {
                temp = mantissa / 10;
                if (mantissa - temp * 10 >= 5)
                    mantissa = temp + 1;
                else
                    mantissa = temp;
                lengthOfFractional -= 1;
            }
            // Strip trailing zeros
            while (lengthOfFractional >= 4 && mantissa == (temp = mantissa / 10000) * 10000)
            {
                mantissa = temp;
                lengthOfFractional -= 4;
            }
            while (lengthOfFractional > 0 && mantissa == (temp = mantissa / 10) * 10)
            {
                mantissa = temp;
                lengthOfFractional -= 1;
            }
        }
        // Write the digits
        if (lengthOfFractional == 0) // No fractional digits: write it as an integer directly
        {
            var bytesWritten = ToUtf8StringUnsafe(mantissa, dest);
            if (bytesWritten > lengthOfInteger && exponent != 0)
            {
                exponent += 1;
                bytesWritten -= 1;
            }
            dest += bytesWritten;
        }
        // If the integer part is 0, write the integer part first.
        // exponent == 0 must be required: in scientific notation (0 < value < 1e-4) the first digit of mantissa is the integer part,
        // taking this branch makes firstNumIndex negative and rolling dest back overwrites the decimal point just written,
        // producing illegal JSON numbers like "04940656458412466E-324". That case must go to the branch below (insert the decimal point after the first digit).
        else if (value < 1 && exponent == 0)
        {
            *dest = (byte) '0';
            *(dest + 1) = (byte) dot_sign;
            dest += 2;
            // The integer is zero, so the decimal point does not need to move; write the fractional part directly
            var fractionalExponent = GetExponent(mantissa);
            var firstNumIndex = lengthOfFractional - (fractionalExponent + 1);
            for (int i = 0; i < firstNumIndex; i++)
            {
                dest[i] = (byte) '0';
            }
            dest += firstNumIndex;
            ToUtf8StringUnsafe(mantissa, dest);
            dest += fractionalExponent + 1;
        }
        else
        {
            // With fractional digits, write as an integer first, then write the decimal point
            ToUtf8StringUnsafe(mantissa, dest);
            dest += lengthOfInteger;
            // Shift the whole fractional part one position to make room for the decimal point
            for (var i = lengthOfFractional; i > 0;)
            {
                dest[i] = dest[--i];
            }
            *dest = (byte) dot_sign;
            dest += lengthOfFractional + 1;
        }
        // If scientific notation is needed, write the exponent part
        if (exponent != 0)
        {
            dest[0] = (byte) exponent_sign;
            if (exponent > 0) {
                dest[1] = (byte) positive_sign;
                dest += 2;
                dest += ToUtf8StringUnsafe((ulong) exponent, dest);
            } else {
                dest[1] = (byte) negative_sign;
                dest += 2;
                dest += ToUtf8StringUnsafe((ulong) (-exponent), dest);
            }
        }
        return (int) (dest - start);
    }
}
