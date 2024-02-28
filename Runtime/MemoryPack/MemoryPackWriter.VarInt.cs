/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 * Description: MemoryPackWriter_VarInt
 */

namespace PeroTools2.Serde.MemoryPack
{
    public ref partial struct MemoryPackWriter<TBufferWriter>
    {
        public void WriteVarInt(uint x)
        {
            if (x <= VarIntCode.max_single_value)
            {
                WriteUnmanaged((sbyte) x);
            }
            else if (x <= ushort.MaxValue)
            {
                WriteUnmanaged(VarIntCode.uint16, (ushort) x);
            }
            else
            {
                WriteUnmanaged(VarIntCode.uint32, x);
            }
        }
    }
}