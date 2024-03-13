/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: SerdeSerializer
 */

using PeroTools2.Serde.Json;

namespace PeroTools2.Serde
{
    public static class PeroSerde
    {
        public static string SerializeToJsonString<T>(T value) where T : ISerdeJsonSerializable
        {
            var bufferWriter = ReusableBufferWriter<char>.Pool.Rent();
            try
            {
                var jsonWriter = new JsonWriterUtf16<ReusableBufferWriter<char>>(bufferWriter);
                jsonWriter.WriteValue(value);
                // string.ctor会负责拷贝内存.
                return new string(bufferWriter.GetWrittenBuffer());
            }
            finally
            {
                ReusableBufferWriter<char>.Pool.Return(bufferWriter);
            }
        }

        public static byte[] SerializeToJsonBytes<T>(T value) where T : ISerdeJsonSerializable
        {
            var bufferWriter = ReusableBufferWriter<byte>.Pool.Rent();
            try
            {
                var jsonWriter = new JsonWriterUtf8<ReusableBufferWriter<byte>>(bufferWriter);
                jsonWriter.WriteValue(value);
                return bufferWriter.CopyToArray();
            }
            finally
            {
                ReusableBufferWriter<byte>.Pool.Return(bufferWriter);
            }
        }
    }
}