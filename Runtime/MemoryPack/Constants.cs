/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 * Description: Constants
 */

using System;

namespace PeroTools2.Serde.MemoryPack
{
    public static class MemoryPackCode
    {
        // Collection Header
        // 0~* is Length, -1 is null
        public const int null_collection = -1;

        // Object/Union Header
        // 0~249 is member count or tag, 250~254 is unused, 255 is null
        public const byte wide_tag = 250; // for Union, 250 is Wide Tag

        // for CircularReference, 250 is ReferenceId Marker, next VarInt id Reference.
        public const byte reference_id = 250;

        public const byte reserved1 = 250;
        public const byte reserved2 = 251;
        public const byte reserved3 = 252;
        public const byte reserved4 = 253;
        public const byte reserved5 = 254;
        public const byte null_object = 255;

        // predefined, C# compiler optimize byte[] as ReadOnlySpan<byte> property.
        internal static ReadOnlySpan<byte> nullCollectionData => new byte[] { 255, 255, 255, 255 }; // -1
        internal static ReadOnlySpan<byte> zeroCollectionData => new byte[] { 0, 0, 0, 0 }; // 0
    }

    internal static class VarIntCode
    {
        public const byte max_single_value = 127;
        public const sbyte min_single_value = -120;

        public const sbyte @byte = -121;
        public const sbyte @sbyte = -122;
        public const sbyte uint16 = -123;
        public const sbyte int16 = -124;
        public const sbyte uint32 = -125;
        public const sbyte int32 = -126;
        public const sbyte uint64 = -127;
        public const sbyte int64 = -128;
    }
}