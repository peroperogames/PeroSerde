/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: MemoryPackSerializer
 */

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PeroTools2.Serde.MemoryPack
{
    public static class MemoryPackSerializer
    {
        [ThreadStatic]
        private static SerializerWriterThreadStaticState s_ThreadStaticState;

        [ThreadStatic]
        private static MemoryPackWriterOptionalState s_ThreadStaticWriterOptionalState;

        public static byte[] Serialize<T>(in T value, MemoryPackSerializerOptions options = null)
        {
            // 如果T是Unmanaged Type, 则可以直接将原始内存写入.
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                var array = new byte[Unsafe.SizeOf<T>()];
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(array.AsSpan()), value);
                return array;
            }
            s_ThreadStaticState ??= new SerializerWriterThreadStaticState();
            var state = s_ThreadStaticState;
            state.Init(options);
            try
            {
                // TODO 
            }
            finally
            {
                state.Reset();
            }
            throw new NotImplementedException();
        }

        private sealed class SerializerWriterThreadStaticState
        {
            public ReusableBufferWriter<byte> bufferWriter = new();
            public MemoryPackWriterOptionalState optionalState = new();

            public void Init(MemoryPackSerializerOptions options)
            {
                optionalState.Init(options);
            }

            public void Reset()
            {
                bufferWriter.Reset();
                optionalState.Reset();
            }
        }
    }
}