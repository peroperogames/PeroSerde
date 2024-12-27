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
        public SerdeException(string message) : base(message) { }

        public SerdeException(string message, Exception innerException) : base(message, innerException) { }

        public static SerdeException ThrowInvalidSurrogateChar(char c) => new SerdeException($"Surrogate char is not supported. Char: {c}");

        public static SerdeException ThrowInvalidParsing() => new SerdeException("Invalid parsing.");

        [DoesNotReturn]
        public static SerdeException ThrowInvalidAdvance() => new SerdeException("Cannot advance past the end of the buffer.");

        [DoesNotReturn]
        public static void ThrowInvalidFloatPoint() =>
            throw new SerdeException("Nan or infinity float point value is invalid in json.");

        [DoesNotReturn]
        public static SerdeException ThrowInvalidEscapedChar(char ch) =>
            new($"Invalid Escaped Char: {ch}");

        [DoesNotReturn]
        public static SerdeException ThrowInvalidJson(string json) => new($"Invalid json: {json.ToString()}");

        [DoesNotReturn]
        public static SerdeException ThrowFormatterNotFound(Type type) => new SerdeException($"Cannot found formatter for type \"{type.Name}\".");
    }
}