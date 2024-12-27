/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/05
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    public interface ISerdeJsonFormatter<T>
    {
        bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        JsonType jsonType { get; }

        int GetMaxRequiredSizeUtf8() => throw new NotImplementedException();
        int GetMaxRequiredSizeUtf16() => throw new NotImplementedException();

        void SerializeUtf8(T value, ref JsonWriterUtf8 writer) => throw new NotImplementedException();
        void SerializeUtf16(T value, ref JsonWriterUtf16 writer) => throw new NotImplementedException();

        unsafe int SerializeUtf16Unsafe(T value, char* dest) => throw new NotImplementedException();
        unsafe int SerializeUtf8Unsafe(T value, byte* dest) => throw new NotImplementedException();

        T DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader) => throw new NotImplementedException();
        T DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader) => throw new NotImplementedException();

        unsafe T DeserializeUtf8Unsafe(byte* src, out int advance) => throw new NotImplementedException();
        unsafe T DeserializeUtf16Unsafe(char* span, out int advance) => throw new NotImplementedException();
    }

    public interface IFixedSerdeJsonFormatter<T> : ISerdeJsonFormatter<T>
    {
        bool isFixedFormatter => true;

        JsonType jsonType { get; }

        int GetMaxRequiredSizeUtf8();
        int GetMaxRequiredSizeUtf16();

        unsafe int SerializeUtf16Unsafe(T value, char* dest);
        unsafe int SerializeUtf8Unsafe(T value, byte* dest);

        unsafe T DeserializeUtf8Unsafe(byte* src, out int advance);
        unsafe T DeserializeUtf16Unsafe(char* span, out int advance);
    }
}