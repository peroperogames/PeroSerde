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
    /// <summary>
    /// Serde 通用异常
    /// </summary>
    public class SerdeException : Exception
    {
        public SerdeException(string message) : base(message) { }

        public SerdeException(string message, Exception innerException) : base(message, innerException) { }

        [DoesNotReturn]
        public static SerdeException ThrowInvalidSurrogateChar(char c) => new($"Surrogate char is not supported. Char: {c}");

        [DoesNotReturn]
        public static SerdeException ThrowInvalidParsing() => new("Invalid parsing.");

        [DoesNotReturn]
        public static SerdeException ThrowInvalidAdvance() => new("Cannot advance past the end of the buffer.");

        [DoesNotReturn]
        public static SerdeException ThrowInvalidFloatPoint() => new("Nan or infinity float point value is invalid in json.");

        [DoesNotReturn]
        public static SerdeException ThrowInvalidEscapedChar(char ch) => new($"Invalid Escaped Char: {ch}");

        [DoesNotReturn]
        public static SerdeException ThrowInvalidJson(string json) => new($"Invalid json: {json}");

        [DoesNotReturn]
        public static SerdeException ThrowFormatterNotFound(Type type) => new($"Cannot found formatter for type \"{type.Name}\".");

        [DoesNotReturn]
        public static SerdeException ThrowNotSupportedType(Type type) => new($"Type of {type} cannot be serialized or deserialized.");
    }
}