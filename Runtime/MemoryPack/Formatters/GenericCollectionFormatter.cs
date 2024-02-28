/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/22
 * Description: GenericCollectionFormatter
 */

using System;
using System.Collections.Generic;

namespace PeroTools2.Serde.MemoryPack.Formatters
{
    public sealed class GenericCollectionFormatter<TCollection, TElement> : MemoryPackFormatter<TCollection>
        where TCollection : ICollection<TElement>, new()
    {
        public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TCollection value)
        {
            if (value == null)
            {
                writer.WriteNullCollectionHeader();
                return;
            }
            var formatter = writer.GetFormatter<TElement>();
            writer.WriteCollectionHeader(value.Count);
            foreach (var element in value)
            {
                var e = element;
                formatter.Serialize(ref writer, ref e);
            }
        }
    }
}