/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/12
 * Description: BuiltInFormatter
 */

using System;

namespace PeroTools2.Serde.Json.Formatter
{
    static partial class SerdeJsonFormatterProvider
    {
        static partial void RegisterBuildInFormatters()
        {
            Register(new GuidFormatter());
        }
    }

    public sealed class GuidFormatter : SerdeJsonFormatter<Guid>
    {
        public override bool isUnsafeFormatter => true;

        public override int GetMaxRequiredSizeUtf8(Guid value) => 38; // 36 + 2引号
        public override int GetMaxRequiredSizeUtf16(Guid value) => 38;

        public override unsafe int SerializeUtf8Unsafe(Guid value, byte* dest)
        {
            dest[0] = (byte) '\"';
            new GuidBits(ref value).WriteUtf8(dest + 1);
            dest[37] = (byte) '\"';
            return 38;
        }

        public override unsafe int SerializeUtf16Unsafe(Guid value, char* dest)
        {
            dest[0] = '\"';
            new GuidBits(ref value).WriteUtf16(dest + 1);
            dest[37] = '\"';
            return 38;
        }
    }
}