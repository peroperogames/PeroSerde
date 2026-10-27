/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Description: 游标路径的集合读取（元素读取器为 struct，避免接口分发）
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json
{
    /// <summary>游标元素读取器。以 struct 实现，配合约束泛型可被内联。</summary>
    public interface ICursorReader<T>
    {
        T Read(ref JsonCursor cursor);
    }

    public struct Int32CursorReader : ICursorReader<int> { public int Read(ref JsonCursor c) => JsonPrimitives.ReadInt32(ref c); }
    public struct Int64CursorReader : ICursorReader<long> { public long Read(ref JsonCursor c) => JsonPrimitives.ReadInt64(ref c); }
    public struct UInt32CursorReader : ICursorReader<uint> { public uint Read(ref JsonCursor c) => JsonPrimitives.ReadUInt32(ref c); }
    public struct UInt64CursorReader : ICursorReader<ulong> { public ulong Read(ref JsonCursor c) => JsonPrimitives.ReadUInt64(ref c); }
    public struct Int16CursorReader : ICursorReader<short> { public short Read(ref JsonCursor c) => JsonPrimitives.ReadInt16(ref c); }
    public struct UInt16CursorReader : ICursorReader<ushort> { public ushort Read(ref JsonCursor c) => JsonPrimitives.ReadUInt16(ref c); }
    public struct ByteCursorReader : ICursorReader<byte> { public byte Read(ref JsonCursor c) => JsonPrimitives.ReadByte(ref c); }
    public struct SByteCursorReader : ICursorReader<sbyte> { public sbyte Read(ref JsonCursor c) => JsonPrimitives.ReadSByte(ref c); }
    public struct SingleCursorReader : ICursorReader<float> { public float Read(ref JsonCursor c) => JsonPrimitives.ReadSingle(ref c); }
    public struct DoubleCursorReader : ICursorReader<double> { public double Read(ref JsonCursor c) => JsonPrimitives.ReadDouble(ref c); }
    public struct DecimalCursorReader : ICursorReader<decimal> { public decimal Read(ref JsonCursor c) => JsonPrimitives.ReadDecimal(ref c); }
    public struct BooleanCursorReader : ICursorReader<bool> { public bool Read(ref JsonCursor c) => JsonPrimitives.ReadBoolean(ref c); }
    public struct CharCursorReader : ICursorReader<char> { public char Read(ref JsonCursor c) => JsonPrimitives.ReadChar(ref c); }
    public struct StringCursorReader : ICursorReader<string> { public string Read(ref JsonCursor c) => JsonPrimitives.ReadString(ref c); }

    /// <summary>游标路径的集合读取器。</summary>
    public static unsafe class JsonCollections
    {
        public static List<T> ReadList<T, TReader>(ref JsonCursor cursor) where TReader : struct, ICursorReader<T>
        {
            cursor.SkipWhiteSpace();
            if (cursor.p >= cursor.end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            if (*cursor.p == (byte) 'n')
            {
                cursor.SkipValue();
                return null;
            }

            cursor.Expect((byte) '[');
            var list = new List<T>();
            if (cursor.TryConsume((byte) ']'))
            {
                return list;
            }

            var reader = default(TReader);
            while (true)
            {
                list.Add(reader.Read(ref cursor));
                if (cursor.TryConsume((byte) ','))
                {
                    continue;
                }

                cursor.Expect((byte) ']');
                return list;
            }
        }

        public static T[] ReadArray<T, TReader>(ref JsonCursor cursor) where TReader : struct, ICursorReader<T>
        {
            cursor.SkipWhiteSpace();
            if (cursor.p >= cursor.end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            if (*cursor.p == (byte) 'n')
            {
                cursor.SkipValue();
                return null;
            }

            cursor.Expect((byte) '[');
            var list = new List<T>();
            if (!cursor.TryConsume((byte) ']'))
            {
                var reader = default(TReader);
                while (true)
                {
                    list.Add(reader.Read(ref cursor));
                    if (cursor.TryConsume((byte) ','))
                    {
                        continue;
                    }

                    cursor.Expect((byte) ']');
                    break;
                }
            }

            return list.ToArray();
        }
    }
}
