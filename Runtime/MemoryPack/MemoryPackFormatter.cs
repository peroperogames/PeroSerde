/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 * Description: MemoryPackFormatter
 */

using System.Buffers;

namespace PeroTools2.Serde.MemoryPack
{
    public interface IMemoryPackFormatterRegister
    {
    }

    public interface IMemoryPackable<T> : IMemoryPackFormatterRegister
    {
    }

    public interface IMemoryPackFormatter
    {
        void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref object value)
            where TBufferWriter : class, IBufferWriter<byte>;
    }

    public interface IMemoryPackFormatter<T>
    {
        void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref T value)
            where TBufferWriter : class, IBufferWriter<byte>;
    }

    public abstract class MemoryPackFormatter<T> : IMemoryPackFormatter<T>, IMemoryPackFormatter
    {
        public abstract void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref T value) where TBufferWriter : class, IBufferWriter<byte>;

        void IMemoryPackFormatter.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref object value)
        {
            var v = value != null ? (T) value : default;
            Serialize(ref writer, ref v);
        }
    }
}