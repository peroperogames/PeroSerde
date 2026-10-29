/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/10/28
 */

using System.Collections.Generic;

namespace PeroTools2.Serde.Json
{
    /// <summary>游标路径的 UTF-16 元素读取器。</summary>
    public interface ICursorReaderUtf16<T>
    {
        T Read(ref JsonCursorUtf16 cursor);
    }

    public struct Int32CursorReaderUtf16 : ICursorReaderUtf16<int>
    {
        public int Read(ref JsonCursorUtf16 c) => JsonReader.ReadInt32(ref c);
    }

    public struct Int64CursorReaderUtf16 : ICursorReaderUtf16<long>
    {
        public long Read(ref JsonCursorUtf16 c) => JsonReader.ReadInt64(ref c);
    }

    public struct UInt32CursorReaderUtf16 : ICursorReaderUtf16<uint>
    {
        public uint Read(ref JsonCursorUtf16 c) => JsonReader.ReadUInt32(ref c);
    }

    public struct UInt64CursorReaderUtf16 : ICursorReaderUtf16<ulong>
    {
        public ulong Read(ref JsonCursorUtf16 c) => JsonReader.ReadUInt64(ref c);
    }

    public struct Int16CursorReaderUtf16 : ICursorReaderUtf16<short>
    {
        public short Read(ref JsonCursorUtf16 c) => JsonReader.ReadInt16(ref c);
    }

    public struct UInt16CursorReaderUtf16 : ICursorReaderUtf16<ushort>
    {
        public ushort Read(ref JsonCursorUtf16 c) => JsonReader.ReadUInt16(ref c);
    }

    public struct ByteCursorReaderUtf16 : ICursorReaderUtf16<byte>
    {
        public byte Read(ref JsonCursorUtf16 c) => JsonReader.ReadByte(ref c);
    }

    public struct SByteCursorReaderUtf16 : ICursorReaderUtf16<sbyte>
    {
        public sbyte Read(ref JsonCursorUtf16 c) => JsonReader.ReadSByte(ref c);
    }

    public struct SingleCursorReaderUtf16 : ICursorReaderUtf16<float>
    {
        public float Read(ref JsonCursorUtf16 c) => JsonReader.ReadSingle(ref c);
    }

    public struct DoubleCursorReaderUtf16 : ICursorReaderUtf16<double>
    {
        public double Read(ref JsonCursorUtf16 c) => JsonReader.ReadDouble(ref c);
    }

    public struct DecimalCursorReaderUtf16 : ICursorReaderUtf16<decimal>
    {
        public decimal Read(ref JsonCursorUtf16 c) => JsonReader.ReadDecimal(ref c);
    }

    public struct BooleanCursorReaderUtf16 : ICursorReaderUtf16<bool>
    {
        public bool Read(ref JsonCursorUtf16 c) => JsonReader.ReadBoolean(ref c);
    }

    public struct CharCursorReaderUtf16 : ICursorReaderUtf16<char>
    {
        public char Read(ref JsonCursorUtf16 c) => JsonReader.ReadChar(ref c);
    }

    public struct StringCursorReaderUtf16 : ICursorReaderUtf16<string>
    {
        public string Read(ref JsonCursorUtf16 c) => JsonReader.ReadString(ref c);
    }

    /// <summary>游标路径的 UTF-16 集合读取器。</summary>
    public static unsafe partial class JsonReader
    {
        public static List<T> ReadList<T, TReader>(ref JsonCursorUtf16 cursor) where TReader : struct, ICursorReaderUtf16<T>
        {
            cursor.SkipWhiteSpace();
            if (cursor.p >= cursor.end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            if (*cursor.p == 'n')
            {
                cursor.SkipValue();
                return null;
            }

            cursor.Expect('[');
            var list = new List<T>();
            if (cursor.TryConsume(']'))
            {
                return list;
            }

            var reader = default(TReader);
            while (true)
            {
                list.Add(reader.Read(ref cursor));
                if (cursor.TryConsume(','))
                {
                    continue;
                }

                cursor.Expect(']');
                return list;
            }
        }

        public static T[] ReadArray<T, TReader>(ref JsonCursorUtf16 cursor) where TReader : struct, ICursorReaderUtf16<T>
        {
            cursor.SkipWhiteSpace();
            if (cursor.p >= cursor.end)
            {
                throw SerdeException.ThrowInvalidJson("Invalid json format.");
            }

            if (*cursor.p == 'n')
            {
                cursor.SkipValue();
                return null;
            }

            cursor.Expect('[');
            var list = new List<T>();
            if (!cursor.TryConsume(']'))
            {
                var reader = default(TReader);
                while (true)
                {
                    list.Add(reader.Read(ref cursor));
                    if (cursor.TryConsume(','))
                    {
                        continue;
                    }

                    cursor.Expect(']');
                    break;
                }
            }

            return list.ToArray();
        }
    }
}