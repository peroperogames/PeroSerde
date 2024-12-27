/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Runtime.CompilerServices;
using PeroTools2.Serde.Json;

namespace PeroTools2.Serde
{
    public static class JsonConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SerializeUtf16<T>(T value)
        {
            var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
            try
            {
                var jsonWriter = new JsonWriterUtf16(bufferWriter);
                jsonWriter.WriteValue(value);
                jsonWriter.Flush();
                return new string(bufferWriter.GetWrittenBuffer());
            }
            finally
            {
                ReusableBufferWriter<char>.Pool.Return(bufferWriter);
            }
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string SerializeUtf16<T>(IReadOnlyList<T> value)
        // {
        //     var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
        //     try
        //     {
        //         var jsonWriter = new JsonWriterUtf16<ReusableBufferWriter<char>>(bufferWriter);
        //         jsonWriter.WriteReadonlyList(value);
        //         jsonWriter.Flush();
        //         return new string(bufferWriter.GetWrittenBuffer());
        //     }
        //     finally
        //     {
        //         ReusableBufferWriter<char>.Pool.Return(bufferWriter);
        //     }
        // }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string SerializeUtf16<T>(IList<T> value)
        // {
        //     var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
        //     try
        //     {
        //         var jsonWriter = new JsonWriterUtf16<ReusableBufferWriter<char>>(bufferWriter);
        //         jsonWriter.WriteList(value);
        //         jsonWriter.Flush();
        //         return new string(bufferWriter.GetWrittenBuffer());
        //     }
        //     finally
        //     {
        //         ReusableBufferWriter<char>.Pool.Return(bufferWriter);
        //     }
        // }
        //
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string SerializeUtf16<TKey, TValue>(IDictionary<TKey, TValue> value)
        // {
        //     var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
        //     try
        //     {
        //         var jsonWriter = new JsonWriterUtf16<ReusableBufferWriter<char>>(bufferWriter);
        //         jsonWriter.WriteDictionary(value);
        //         jsonWriter.Flush();
        //         return new string(bufferWriter.GetWrittenBuffer());
        //     }
        //     finally
        //     {
        //         ReusableBufferWriter<char>.Pool.Return(bufferWriter);
        //     }
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SerializeUtf8<T>(T value)
        {
            var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
            try
            {
                var jsonWriter = new JsonWriterUtf8(bufferWriter);
                jsonWriter.WriteValue(value);
                jsonWriter.Flush();
                return bufferWriter.CopyToArray();
            }
            finally
            {
                ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
            }
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static byte[] SerializeUtf8<T>(IList<T> value)
        // {
        //     var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
        //     try
        //     {
        //         var jsonWriter = new JsonWriterUtf8(bufferWriter);
        //         jsonWriter.WriteValue(value);
        //         jsonWriter.Flush();
        //         return bufferWriter.CopyToArray();
        //     }
        //     finally
        //     {
        //         ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
        //     }
        // }
        //
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static byte[] SerializeUtf8<T>(IReadOnlyList<T> value)
        // {
        //     var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
        //     try
        //     {
        //         var jsonWriter = new JsonWriterUtf8(bufferWriter);
        //         jsonWriter.WriteReadonlyList(value);
        //         jsonWriter.Flush();
        //         return bufferWriter.CopyToArray();
        //     }
        //     finally
        //     {
        //         ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
        //     }
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeUtf16<T>(ReadOnlySpan<char> json) => new JsonReaderUtf16(json).ReadValue<T>(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeUtf8<T>(ReadOnlySpan<byte> json) => new JsonReaderUtf8(json).ReadValue<T>(false);
    }
}