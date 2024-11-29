namespace PeroTools2.Serde.Json.Formatter
{
    public sealed class StringFormatter : SerdeJsonFormatter<string>
    {
        public override bool isUnsafeFormatter => false;

        // public override void SerializeUtf8<TBufferWriter>(string value, ref JsonWriterUtf8<TBufferWriter> writer) => writer.WriteString(value);

        public override void SerializeUtf16<TBufferWriter>(string value, ref JsonWriterUtf16<TBufferWriter> writer) => writer.WriteString(value);
    }
}