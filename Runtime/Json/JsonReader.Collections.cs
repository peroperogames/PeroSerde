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
        T Read(ref JsonCursorUtf8 cursor);
    }

    public struct Int32CursorReader : ICursorReader<int>
    {
        public int Read(ref JsonCursorUtf8 c) => JsonReader.ReadInt32(ref c);
    }

    public struct Int64CursorReader : ICursorReader<long>
    {
        public long Read(ref JsonCursorUtf8 c) => JsonReader.ReadInt64(ref c);
    }

    public struct UInt32CursorReader : ICursorReader<uint>
    {
        public uint Read(ref JsonCursorUtf8 c) => JsonReader.ReadUInt32(ref c);
    }

    public struct UInt64CursorReader : ICursorReader<ulong>
    {
        public ulong Read(ref JsonCursorUtf8 c) => JsonReader.ReadUInt64(ref c);
    }

    public struct Int16CursorReader : ICursorReader<short>
    {
        public short Read(ref JsonCursorUtf8 c) => JsonReader.ReadInt16(ref c);
    }

    public struct UInt16CursorReader : ICursorReader<ushort>
    {
        public ushort Read(ref JsonCursorUtf8 c) => JsonReader.ReadUInt16(ref c);
    }

    public struct ByteCursorReader : ICursorReader<byte>
    {
        public byte Read(ref JsonCursorUtf8 c) => JsonReader.ReadByte(ref c);
    }

    public struct SByteCursorReader : ICursorReader<sbyte>
    {
        public sbyte Read(ref JsonCursorUtf8 c) => JsonReader.ReadSByte(ref c);
    }

    public struct SingleCursorReader : ICursorReader<float>
    {
        public float Read(ref JsonCursorUtf8 c) => JsonReader.ReadSingle(ref c);
    }

    public struct DoubleCursorReader : ICursorReader<double>
    {
        public double Read(ref JsonCursorUtf8 c) => JsonReader.ReadDouble(ref c);
    }

    public struct DecimalCursorReader : ICursorReader<decimal>
    {
        public decimal Read(ref JsonCursorUtf8 c) => JsonReader.ReadDecimal(ref c);
    }

    public struct BooleanCursorReader : ICursorReader<bool>
    {
        public bool Read(ref JsonCursorUtf8 c) => JsonReader.ReadBoolean(ref c);
    }

    public struct CharCursorReader : ICursorReader<char>
    {
        public char Read(ref JsonCursorUtf8 c) => JsonReader.ReadChar(ref c);
    }

    public struct StringCursorReader : ICursorReader<string>
    {
        public string Read(ref JsonCursorUtf8 c) => JsonReader.ReadString(ref c);
    }

    /// <summary>游标路径的集合读取器。</summary>
    public static unsafe partial class JsonReader
    {
        public static List<T> ReadList<T, TReader>(ref JsonCursorUtf8 cursor) where TReader : struct, ICursorReader<T>
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

        public static T[] ReadArray<T, TReader>(ref JsonCursorUtf8 cursor) where TReader : struct, ICursorReader<T>
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