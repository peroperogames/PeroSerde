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
    public sealed class SerdeException : Exception
    {
        internal SerdeException(string message) : base(message) { }

        internal SerdeException(string message, Exception innerException) : base(message, innerException) { }

        [DoesNotReturn]
        internal static SerdeException ThrowInvalidSurrogateChar(char c) => new($"Surrogate char is not supported. Char: {c}");

        [DoesNotReturn]
        internal static SerdeException ThrowInvalidParsing() => new("Invalid parsing.");

        [DoesNotReturn]
        internal static SerdeException ThrowInvalidAdvance() => new("Cannot advance past the end of the buffer.");

        [DoesNotReturn]
        internal static SerdeException ThrowInvalidFloatPoint() => new("Nan or infinity float point value is invalid in json.");

        [DoesNotReturn]
        internal static SerdeException ThrowInvalidEscapedChar(char ch) => new($"Invalid Escaped Char: {ch}");

        [DoesNotReturn]
        internal static SerdeException ThrowInvalidJson(string json) => new($"Invalid json: {json}");

        [DoesNotReturn]
        internal static SerdeException ThrowFormatterNotFound(Type type) => new($"Cannot found formatter for type \"{type.Name}\".");

        [DoesNotReturn]
        internal static SerdeException ThrowNotSupportedType(Type type) => new($"Type of {type} cannot be serialized or deserialized.");
    }
}