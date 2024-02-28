/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: MemoryPackSerializerOptions
 */

namespace PeroTools2.Serde.MemoryPack
{
    public sealed record MemoryPackSerializerOptions
    {
        // ReSharper disable once InconsistentNaming
        public static MemoryPackSerializerOptions Default => UTF8;

        // ReSharper disable once InconsistentNaming
        public static readonly MemoryPackSerializerOptions UTF8 = new()
        {
            stringEncoding = StringEncoding.UTF8,
        };

        // ReSharper disable once InconsistentNaming
        public static readonly MemoryPackSerializerOptions UTF16 = new()
        {
            stringEncoding = StringEncoding.UTF16,
        };

        public StringEncoding stringEncoding { get; init; }
    }

    public enum StringEncoding : byte
    {
        UTF16,
        UTF8,
    }
}