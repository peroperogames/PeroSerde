/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/05
 * Description: IJsonFormatter
 */

using System;
using System.Buffers;

namespace PeroTools2.Serde.Json
{
    public interface IUnsafeSerdeJsonFormatterUtf8<T>
    {
        int GetMaxRequiredSizeUtf8();
        unsafe int SerializeUtf8Unsafe(T value, byte* dest);
    }

    public interface IUnsafeSerdeJsonFormatterUtf16<T>
    {
        int GetMaxRequiredSizeUtf16();
        unsafe int SerializeUtf16Unsafe(T value, char* dest);
    }

    public interface IUnsafeSerdeJsonFormatter<T> : IUnsafeSerdeJsonFormatterUtf8<T>, IUnsafeSerdeJsonFormatterUtf16<T>
    {
    }

    public interface ISerdeJsonFormatterUtf8<T>
    {
        void SerializeUtf8<TBufferWriter>(T value, ref JsonWriterUtf8<TBufferWriter> writer) where TBufferWriter : class, IBufferWriter<byte>;
    }

    public interface ISerdeJsonFormatterUtf16<T>
    {
        void SerializeUtf16<TBufferWriter>(T value, ref JsonWriterUtf16<TBufferWriter> writer) where TBufferWriter : class, IBufferWriter<char>;
    }

    public interface ISerdeJsonFormatter<T> : ISerdeJsonFormatterUtf8<T>, ISerdeJsonFormatterUtf16<T>
    {
    }

    public abstract class SerdeJsonFormatter<T> : ISerdeJsonFormatter<T>, IUnsafeSerdeJsonFormatter<T>
    {
        public abstract bool isUnsafeFormatter { get; }

        public virtual int GetMaxRequiredSizeUtf8() => throw new NotImplementedException();
        public virtual unsafe int SerializeUtf8Unsafe(T value, byte* dest) => throw new NotImplementedException();

        public virtual int GetMaxRequiredSizeUtf16() => throw new NotImplementedException();
        public virtual unsafe int SerializeUtf16Unsafe(T value, char* dest) => throw new NotImplementedException();

        public virtual void SerializeUtf8<TBufferWriter>(T value, ref JsonWriterUtf8<TBufferWriter> writer) where TBufferWriter : class, IBufferWriter<byte>
        {
            throw new NotImplementedException();
        }

        public virtual void SerializeUtf16<TBufferWriter>(T value, ref JsonWriterUtf16<TBufferWriter> writer) where TBufferWriter : class, IBufferWriter<char>
        {
            throw new NotImplementedException();
        }
    }

    public interface ISerdeJsonSerializable
    {
    }
}