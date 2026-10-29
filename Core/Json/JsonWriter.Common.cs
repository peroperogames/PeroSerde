using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json;

public ref struct JsonWriterUtf8(ReusableBufferWriter<byte> writer)
{
    private Span<byte> m_BufferRef = default;
    private int m_AdvancedCount = 0;
    private int m_WrittenCount = 0;

    #region Buffer Op

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref byte GetSpanRef(int sizeHint)
    {
        if (m_BufferRef.Length < sizeHint)
        {
            RequestNewBuffer(sizeHint);
        }
        return ref m_BufferRef.GetPinnableReference();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<byte> GetSpan(int sizeHint)
    {
        if (m_BufferRef.Length < sizeHint)
        {
            RequestNewBuffer(sizeHint);
        }
        return m_BufferRef;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void RequestNewBuffer(int sizeHint)
    {
        if (m_AdvancedCount != 0)
        {
            writer.Advance(m_AdvancedCount);
            m_AdvancedCount = 0;
        }
        m_BufferRef = writer.GetSpan(sizeHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Advance(int count)
    {
        if (count == 0) return;
        if (m_BufferRef.Length - count < 0)
        {
            SerdeException.ThrowInvalidAdvance();
        }
        m_BufferRef = m_BufferRef.Slice(count);
        m_AdvancedCount += count;
        m_WrittenCount += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Flush()
    {
        if (m_AdvancedCount != 0)
        {
            writer.Advance(m_AdvancedCount);
            m_AdvancedCount = 0;
        }
        m_BufferRef = default;
        m_WrittenCount = 0;
    }

    #endregion

    #region Write Value

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteNamedValue(ReadOnlySpan<char> key, ReadOnlySpan<byte> value, bool writeComma = false)
    {
        var requiredSize = key.Length + value.Length + 3 + (writeComma ? 1 : 0); // 3 is the size of "" and :
        fixed (byte* startPtr = GetSpan(requiredSize))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteStringNoEscapeUtf8(destPtr, key);
            destPtr += JsonWriter.WriteKeyEndingUtf8(destPtr);
            value.CopyTo(new Span<byte>(destPtr, value.Length));
            destPtr += value.Length;
            if (writeComma)
            {
                destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
            }
            Advance((int) (destPtr - startPtr));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteNamedValue<T>(ReadOnlySpan<char> key, T value, bool writeComma = false)
    {
        var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
        if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
        {
            WriteNamedValue(key, value, fixedFormatter, writeComma);
            return;
        }
        if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
        {
            WriteNamedValue(key, value, unfixedFormatter, writeComma);
            return;
        }
        throw SerdeException.ThrowFormatterNotFound(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteNamedValue<T>(ReadOnlySpan<char> key, T value, ISerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        fixed (byte* startPtr = GetSpan(key.Length + 2))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteStringNoEscapeUtf8(destPtr, key);
            destPtr += JsonWriter.WriteKeyEndingUtf8(destPtr);
            Advance((int) (destPtr - startPtr));
        }
        formatter.SerializeUtf8(value, ref this);
        if (writeComma)
            WriteValueEnding();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteNamedValue<T>(ReadOnlySpan<char> key, T value, IFixedSerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        var requiredSize = key.Length + formatter.GetMaxRequiredSizeUtf8() + 3 + (writeComma ? 1 : 0); // 3 is the size of "" and :
        fixed (byte* startPtr = GetSpan(requiredSize))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteStringNoEscapeUtf8(destPtr, key);
            destPtr += JsonWriter.WriteKeyEndingUtf8(destPtr);
            destPtr += formatter.SerializeUtf8Unsafe(value, destPtr);
            if (writeComma)
            {
                destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
            }
            Advance((int) (destPtr - startPtr));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteValue(ReadOnlySpan<byte> value, bool writeComma = false)
    {
        var requiredSize = value.Length + (writeComma ? 1 : 0); // 3 is the size of "" and :
        fixed (byte* startPtr = GetSpan(requiredSize))
        {
            var destPtr = startPtr;
            value.CopyTo(new Span<byte>(destPtr, value.Length));
            destPtr += value.Length;
            if (writeComma)
            {
                destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
            }
            Advance((int) (destPtr - startPtr));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValue<T>(T value, bool writeComma = false)
    {
        var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
        if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
        {
            WriteValue(value, fixedFormatter, writeComma);
            return;
        }
        if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
        {
            WriteValue(value, unfixedFormatter, writeComma);
            return;
        }
        throw SerdeException.ThrowFormatterNotFound(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValue<T>(T value, ISerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        formatter.SerializeUtf8(value, ref this);
        if (writeComma) WriteValueEnding();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteValue<T>(T value, IFixedSerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        var commaAdvance = writeComma ? 1 : 0;
        var requiredSize = formatter.GetMaxRequiredSizeUtf8() + commaAdvance;
        fixed (byte* destPtr = GetSpan(requiredSize))
        {
            var bytesWritten = formatter.SerializeUtf8Unsafe(value, destPtr);
            if (writeComma)
                JsonWriter.WriteValueEndingUtf8(destPtr);
            Advance(bytesWritten + commaAdvance);
        }
    }

    #endregion

    #region Write Collection

    internal unsafe void WriteCollection<T>(ICollection<T> value, IFixedSerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        if (value.Count == 0)
        {
            var dest = GetSpan(2);
            dest[0] = (byte) '[';
            dest[1] = (byte) ']';
            Advance(2);
            return;
        }
        // +1 is the comma, +2 is the [ and ]
        var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf8() + 1) + 2 + (writeComma ? 0 : 1);
        fixed (byte* startPtr = GetSpan(requiredLength))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteBeginArrayUtf8(destPtr);
            if (value is IList<T> list)
            {
                for (var i = 0; i < value.Count; i++)
                {
                    destPtr += formatter.SerializeUtf8Unsafe(list[i], destPtr);
                    destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
                }
            }
            else
            {
                foreach (var item in value)
                {
                    destPtr += formatter.SerializeUtf8Unsafe(item, destPtr);
                    destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
                }
            }
            // Overwrite the last comma
            JsonWriter.WriteEndArrayUtf8(destPtr - 1);
            if (writeComma)
                destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
            Advance((int) (destPtr - startPtr));
        }
    }

    internal void WriteCollection<T>(ICollection<T> value, ISerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        if (value.Count == 0)
        {
            var dest = GetSpan(2);
            dest[0] = (byte) '[';
            dest[1] = (byte) ']';
            Advance(2);
            return;
        }

        WriteBeginArray();
        if (value is IList<T> list)
        {
            for (var i = 0; i < value.Count - 1; i++)
            {
                formatter.SerializeUtf8(list[i], ref this);
                WriteValueEnding();
            }
            formatter.SerializeUtf8(list[^1], ref this);
        }
        else
        {
            var maxIndex = value.Count - 1;
            var index = 0;
            foreach (var item in value)
            {
                formatter.SerializeUtf8(item, ref this);
                if (index++ != maxIndex)
                {
                    WriteValueEnding();
                }
            }
        }

        WriteEndArray();
        if (writeComma)
        {
            WriteValueEnding();
        }
    }

    internal unsafe void WriteReadonlyCollection<T>(IReadOnlyCollection<T> value, IFixedSerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        if (value.Count == 0)
        {
            var dest = GetSpan(2);
            dest[0] = (byte) '[';
            dest[1] = (byte) ']';
            Advance(2);
            return;
        }
        // +1 is the comma, +2 is the [ and ]
        var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf8() + 1) + 2 + (writeComma ? 0 : 1);
        fixed (byte* startPtr = GetSpan(requiredLength))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteBeginArrayUtf8(destPtr);
            if (value is IList<T> list)
            {
                for (var i = 0; i < value.Count; i++)
                {
                    destPtr += formatter.SerializeUtf8Unsafe(list[i], destPtr);
                    destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
                }
            }
            else
            {
                foreach (var item in value)
                {
                    destPtr += formatter.SerializeUtf8Unsafe(item, destPtr);
                    destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
                }
            }
            // Overwrite the last comma
            JsonWriter.WriteEndArrayUtf8(destPtr - 1);
            if (writeComma)
                destPtr += JsonWriter.WriteValueEndingUtf8(destPtr);
            Advance((int) (destPtr - startPtr));
        }
    }

    internal void WriteReadonlyCollection<T>(IReadOnlyCollection<T> value, ISerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        if (value.Count == 0)
        {
            var dest = GetSpan(2);
            dest[0] = (byte) '[';
            dest[1] = (byte) ']';
            Advance(2);
            return;
        }

        WriteBeginArray();
        if (value is IList<T> list)
        {
            for (var i = 0; i < value.Count - 1; i++)
            {
                formatter.SerializeUtf8(list[i], ref this);
                WriteValueEnding();
            }
            formatter.SerializeUtf8(list[^1], ref this);
        }
        else
        {
            var maxIndex = value.Count - 1;
            var index = 0;
            foreach (var item in value)
            {
                formatter.SerializeUtf8(item, ref this);
                if (index++ != maxIndex)
                {
                    WriteValueEnding();
                }
            }
        }

        WriteEndArray();
        if (writeComma)
        {
            WriteValueEnding();
        }
    }

    #endregion

    #region Write Literal

    /// <summary>Writes a string</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteString(ReadOnlySpan<char> value)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        fixed (byte* desPtr = GetSpan(Encoding.UTF8.GetMaxByteCount(value.Length) + 2))
        {
            var length = JsonWriter.WriteDoubleQuoteUtf8(desPtr);
            length += JsonWriter.WriteStringValueUtf8(value, desPtr + length);
            length += JsonWriter.WriteDoubleQuoteUtf8(desPtr + length);
            Advance(length);
        }
    }

    /// <summary>Writes a string</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteStringWithoutQuote(ReadOnlySpan<char> value)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        fixed (byte* desPtr = GetSpan(Encoding.UTF8.GetMaxByteCount(value.Length)))
        {
            Advance(JsonWriter.WriteStringValueUtf8(value, desPtr));
        }
    }

    /// <summary>Writes an ASCII colon</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteKeyEnding()
    {
        GetSpanRef(1) = (byte) ':';
        Advance(1);
    }

    /// <summary>Writes an ASCII comma</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValueEnding()
    {
        GetSpanRef(1) = (byte) ',';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDoubleQuote()
    {
        GetSpanRef(1) = (byte) '"';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpace()
    {
        GetSpanRef(1) = (byte) ' ';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBeginObject()
    {
        GetSpanRef(1) = (byte) '{';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEndObject()
    {
        GetSpanRef(1) = (byte) '}';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBeginArray()
    {
        GetSpanRef(1) = (byte) '[';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEndArray()
    {
        GetSpanRef(1) = (byte) ']';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteAscii(char value)
    {
        GetSpanRef(1) = (byte) value;
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNull()
    {
        var dest = GetSpan(4);
        dest[0] = (byte) 'n';
        dest[1] = (byte) 'u';
        dest[2] = (byte) 'l';
        dest[3] = (byte) 'l';
        Advance(4);
    }

    #endregion
}

public ref struct JsonWriterUtf16(ReusableBufferWriter<char> writer)
{
    private Span<char> m_BufferRef = default;
    private int m_AdvancedCount = 0;
    private int m_WrittenCount = 0;

    #region Buffer Op

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref char GetSpanRef(int sizeHint)
    {
        if (m_BufferRef.Length < sizeHint)
        {
            RequestNewBuffer(sizeHint);
        }
        return ref m_BufferRef.GetPinnableReference();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<char> GetSpan(int sizeHint)
    {
        if (m_BufferRef.Length < sizeHint)
        {
            RequestNewBuffer(sizeHint);
        }
        return m_BufferRef;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void RequestNewBuffer(int sizeHint)
    {
        if (m_AdvancedCount != 0)
        {
            writer.Advance(m_AdvancedCount);
            m_AdvancedCount = 0;
        }
        m_BufferRef = writer.GetSpan(sizeHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Advance(int count)
    {
        if (count == 0) return;
        if (m_BufferRef.Length - count < 0)
        {
            SerdeException.ThrowInvalidAdvance();
        }
        m_BufferRef = m_BufferRef.Slice(count);
        m_AdvancedCount += count;
        m_WrittenCount += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Flush()
    {
        if (m_AdvancedCount != 0)
        {
            writer.Advance(m_AdvancedCount);
            m_AdvancedCount = 0;
        }
        m_BufferRef = default;
        m_WrittenCount = 0;
    }

    #endregion

    #region Write Value

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteNamedValue(ReadOnlySpan<char> key, ReadOnlySpan<char> value, bool writeComma = false)
    {
        var requiredSize = key.Length + value.Length + 3 + (writeComma ? 1 : 0); // 3 is the size of "" and :
        fixed (char* startPtr = GetSpan(requiredSize))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteStringNoEscapeUtf16(destPtr, key);
            destPtr += JsonWriter.WriteKeyEndingUtf16(destPtr);
            value.CopyTo(new Span<char>(destPtr, value.Length));
            destPtr += value.Length;
            if (writeComma)
            {
                destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
            }
            Advance((int) (destPtr - startPtr));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteNamedValue<T>(ReadOnlySpan<char> key, T value, bool writeComma = false)
    {
        var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
        if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
        {
            WriteNamedValue(key, value, fixedFormatter, writeComma);
            return;
        }
        if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
        {
            WriteNamedValue(key, value, unfixedFormatter, writeComma);
            return;
        }
        throw SerdeException.ThrowFormatterNotFound(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteNamedValue<T>(ReadOnlySpan<char> key, T value, ISerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        fixed (char* startPtr = GetSpan(key.Length + 2))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteStringNoEscapeUtf16(destPtr, key);
            destPtr += JsonWriter.WriteKeyEndingUtf16(destPtr);
            Advance((int) (destPtr - startPtr));
        }
        formatter.SerializeUtf16(value, ref this);
        if (writeComma)
            WriteValueEnding();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteNamedValue<T>(ReadOnlySpan<char> key, T value, IFixedSerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        var requiredSize = key.Length + formatter.GetMaxRequiredSizeUtf16() + 3 + (writeComma ? 1 : 0); // 3 is the size of "" and :
        fixed (char* startPtr = GetSpan(requiredSize))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteStringNoEscapeUtf16(destPtr, key);
            destPtr += JsonWriter.WriteKeyEndingUtf16(destPtr);
            destPtr += formatter.SerializeUtf16Unsafe(value, destPtr);
            if (writeComma)
            {
                destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
            }
            Advance((int) (destPtr - startPtr));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteValue(ReadOnlySpan<char> value, bool writeComma = false)
    {
        var requiredSize = value.Length + (writeComma ? 1 : 0); // 3 is the size of "" and :
        fixed (char* startPtr = GetSpan(requiredSize))
        {
            var destPtr = startPtr;
            value.CopyTo(new Span<char>(destPtr, value.Length));
            destPtr += value.Length;
            if (writeComma)
            {
                destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
            }
            Advance((int) (destPtr - startPtr));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValue<T>(T value, bool writeComma = false)
    {
        var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
        if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
        {
            WriteValue(value, fixedFormatter, writeComma);
            return;
        }
        if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
        {
            WriteValue(value, unfixedFormatter, writeComma);
            return;
        }
        throw SerdeException.ThrowFormatterNotFound(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValue<T>(T value, ISerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        formatter.SerializeUtf16(value, ref this);
        if (writeComma) WriteValueEnding();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteValue<T>(T value, IFixedSerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        var commaAdvance = writeComma ? 1 : 0;
        var requiredSize = formatter.GetMaxRequiredSizeUtf16() + commaAdvance;
        fixed (char* destPtr = GetSpan(requiredSize))
        {
            var charsWritten = formatter.SerializeUtf16Unsafe(value, destPtr);
            if (writeComma)
                JsonWriter.WriteValueEndingUtf16(destPtr);
            Advance(charsWritten + commaAdvance);
        }
    }

    #endregion

    #region Write Collection

    internal unsafe void WriteCollection<T>(ICollection<T> value, IFixedSerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        if (value.Count == 0)
        {
            var dest = GetSpan(2);
            dest[0] = '[';
            dest[1] = ']';
            Advance(2);
            return;
        }
        // +1 is the comma, +2 is the [ and ]
        var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf16() + 1) + 2 + (writeComma ? 0 : 1);
        fixed (char* startPtr = GetSpan(requiredLength))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteBeginArrayUtf16(destPtr);
            if (value is IList<T> list)
            {
                for (var i = 0; i < value.Count; i++)
                {
                    destPtr += formatter.SerializeUtf16Unsafe(list[i], destPtr);
                    destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
                }
            }
            else
            {
                foreach (var item in value)
                {
                    destPtr += formatter.SerializeUtf16Unsafe(item, destPtr);
                    destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
                }
            }
            // Overwrite the last comma
            JsonWriter.WriteEndArrayUtf16(destPtr - 1);
            if (writeComma)
                destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
            Advance((int) (destPtr - startPtr));
        }
    }

    internal void WriteCollection<T>(ICollection<T> value, ISerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        if (value.Count == 0)
        {
            var dest = GetSpan(2);
            dest[0] = '[';
            dest[1] = ']';
            Advance(2);
            return;
        }

        WriteBeginArray();
        if (value is IList<T> list)
        {
            for (var i = 0; i < value.Count - 1; i++)
            {
                formatter.SerializeUtf16(list[i], ref this);
                WriteValueEnding();
            }
            formatter.SerializeUtf16(list[^1], ref this);
        }
        else
        {
            var maxIndex = value.Count - 1;
            var index = 0;
            foreach (var item in value)
            {
                formatter.SerializeUtf16(item, ref this);
                if (index++ != maxIndex)
                {
                    WriteValueEnding();
                }
            }
        }

        WriteEndArray();
        if (writeComma)
        {
            WriteValueEnding();
        }
    }

    internal unsafe void WriteReadonlyCollection<T>(IReadOnlyCollection<T> value, IFixedSerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        if (value.Count == 0)
        {
            var dest = GetSpan(2);
            dest[0] = '[';
            dest[1] = ']';
            Advance(2);
            return;
        }
        // +1 is the comma, +2 is the [ and ]
        var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf16() + 1) + 2 + (writeComma ? 0 : 1);
        fixed (char* startPtr = GetSpan(requiredLength))
        {
            var destPtr = startPtr;
            destPtr += JsonWriter.WriteBeginArrayUtf16(destPtr);
            if (value is IList<T> list)
            {
                for (var i = 0; i < value.Count; i++)
                {
                    destPtr += formatter.SerializeUtf16Unsafe(list[i], destPtr);
                    destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
                }
            }
            else
            {
                foreach (var item in value)
                {
                    destPtr += formatter.SerializeUtf16Unsafe(item, destPtr);
                    destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
                }
            }
            // Overwrite the last comma
            JsonWriter.WriteEndArrayUtf16(destPtr - 1);
            if (writeComma)
                destPtr += JsonWriter.WriteValueEndingUtf16(destPtr);
            Advance((int) (destPtr - startPtr));
        }
    }

    internal void WriteReadonlyCollection<T>(IReadOnlyCollection<T> value, ISerdeJsonFormatter<T> formatter, bool writeComma = false)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        if (value.Count == 0)
        {
            var dest = GetSpan(2);
            dest[0] = '[';
            dest[1] = ']';
            Advance(2);
            return;
        }

        WriteBeginArray();
        if (value is IList<T> list)
        {
            for (var i = 0; i < value.Count - 1; i++)
            {
                formatter.SerializeUtf16(list[i], ref this);
                WriteValueEnding();
            }
            formatter.SerializeUtf16(list[^1], ref this);
        }
        else
        {
            var maxIndex = value.Count - 1;
            var index = 0;
            foreach (var item in value)
            {
                formatter.SerializeUtf16(item, ref this);
                if (index++ != maxIndex)
                {
                    WriteValueEnding();
                }
            }
        }

        WriteEndArray();
        if (writeComma)
        {
            WriteValueEnding();
        }
    }

    #endregion

    #region Write Literal

    /// <summary>Writes a string</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteString(ReadOnlySpan<char> value)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        fixed (char* desPtr = GetSpan(value.Length + 2))
        {
            var length = JsonWriter.WriteDoubleQuoteUtf16(desPtr);
            length += JsonWriter.WriteStringValueUtf16(value, desPtr + length);
            length += JsonWriter.WriteDoubleQuoteUtf16(desPtr + length);
            Advance(length);
        }
    }

    /// <summary>Writes a string</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteStringWithoutQuote(ReadOnlySpan<char> value)
    {
        if (value == null)
        {
            WriteNull();
            return;
        }
        fixed (char* desPtr = GetSpan(value.Length))
        {
            Advance(JsonWriter.WriteStringValueUtf16(value, desPtr));
        }
    }

    /// <summary>Writes an ASCII colon</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteKeyEnding()
    {
        GetSpanRef(1) = ':';
        Advance(1);
    }

    /// <summary>Writes an ASCII comma</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValueEnding()
    {
        GetSpanRef(1) = ',';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDoubleQuote()
    {
        GetSpanRef(1) = '"';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpace()
    {
        GetSpanRef(1) = ' ';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBeginObject()
    {
        GetSpanRef(1) = '{';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEndObject()
    {
        GetSpanRef(1) = '}';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBeginArray()
    {
        GetSpanRef(1) = '[';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEndArray()
    {
        GetSpanRef(1) = ']';
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteChar(char value)
    {
        GetSpanRef(1) = value;
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNull()
    {
        var dest = GetSpan(4);
        dest[0] = 'n';
        dest[1] = 'u';
        dest[2] = 'l';
        dest[3] = 'l';
        Advance(4);
    }

    #endregion
}