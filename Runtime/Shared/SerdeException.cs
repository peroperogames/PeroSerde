/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/26
 * Description: SerdeException
 */

using System;
using System.Diagnostics.CodeAnalysis;

namespace PeroTools2.Serde
{
    public class SerdeException : Exception
    {
        public SerdeException(string message) : base(message)
        {
        }

        public SerdeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        [DoesNotReturn]
        public static void ThrowInvalidAdvance() =>
            throw new SerdeException("Cannot advance past the end of the buffer.");

        [DoesNotReturn]
        public static void ThrowInvalidFloatPoint() =>
            throw new SerdeException("Nan or infinity float point value is invalid in json.");

        public static SerdeException ThrowInvalidEscapedChar(char ch) =>
            new($"Invalid Escaped Char: {ch}");

        [DoesNotReturn]
        public static void ThrowFormatterNotFound(Type type) =>
            throw new SerdeException($"Cannot found formatter for type \"{type.Name}\".");
    }
}