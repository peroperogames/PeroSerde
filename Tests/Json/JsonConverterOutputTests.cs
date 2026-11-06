/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/06
 */

using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeroTools2.Serde.Json.Tests;

[SerdeJsonSerializable]
public partial class OutputTargetModel
{
    public int id;
    public string label;
}

/// <summary>
/// Round-trip tests for the <see cref="JsonConverter"/> overloads that write into a caller supplied target
/// (TextWriter / Stream / IBufferWriter / BinaryWriter, synchronous and asynchronous)
/// </summary>
public sealed class JsonConverterOutputTests
{
    private static OutputTargetModel CreateModel() => new() { id = 7, label = "七" };

    private sealed class AsyncStringWriter : StringWriter
    {
        public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            // complete asynchronously, so the caller really awaits instead of running synchronously to the end
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            Write(buffer.Span);
        }
    }

    private sealed class AsyncMemoryStream : MemoryStream
    {
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            await base.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
    }

    private sealed class CancelingStream : MemoryStream
    {
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }
            return base.WriteAsync(buffer, cancellationToken);
        }
    }

    private sealed class CancelingWriter : StringWriter
    {
        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }
            return base.WriteAsync(buffer, cancellationToken);
        }
    }

    private sealed class ThrowingWriter : StringWriter
    {
        public override void Write(ReadOnlySpan<char> buffer) => throw new InvalidOperationException("write failed");
    }

    private sealed class ThrowingStream : MemoryStream
    {
        public override void Write(ReadOnlySpan<byte> buffer) => throw new InvalidOperationException("write failed");
    }

    [Test]
    public void Utf16TextWriterSyncTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF16.Serialize(model);
        using var writer = new StringWriter();

        JsonConverter.UTF16.Serialize(model, writer);

        Assert.AreEqual(expected, writer.ToString());
        Assert.AreEqual(model.label, JsonConverter.UTF16.Deserialize<OutputTargetModel>(writer.ToString()).label);
    }

    [Test]
    public async Task Utf16TextWriterAsyncTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF16.Serialize(model);
        using var writer = new AsyncStringWriter();

        await JsonConverter.UTF16.SerializeAsync(model, writer);

        Assert.AreEqual(expected, writer.ToString());
    }

    [Test]
    public async Task Utf16TextWriterFormatterTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF16.Serialize(model);
        var formatter = SerdeJsonFormatterProvider.GetFormatter<OutputTargetModel>();
        using var sync = new StringWriter();
        using var async = new AsyncStringWriter();

        JsonConverter.UTF16.Serialize(model, formatter, sync);
        // awaited instead of blocked on, so a writer that captures the caller synchronization context cannot deadlock
        await JsonConverter.UTF16.SerializeAsync(model, formatter, async);

        Assert.AreEqual(expected, sync.ToString());
        Assert.AreEqual(expected, async.ToString());
    }

    [Test]
    public void Utf16TextWriterFixedFormatterTest()
    {
        using var writer = new StringWriter();

        JsonConverter.UTF16.Serialize(42, SerdeJsonFormatterProvider.GetFixedFormatter<int>(), writer);

        Assert.AreEqual("42", writer.ToString());
    }

    [Test]
    public void Utf8StreamFormatterTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF8.Serialize(model);
        var formatter = SerdeJsonFormatterProvider.GetFormatter<OutputTargetModel>();
        using var stream = new MemoryStream();

        JsonConverter.UTF8.Serialize(model, formatter, stream);

        Assert.AreEqual(expected, stream.ToArray());
    }

    [Test]
    public void Utf16BufferWriterSyncTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF16.Serialize(model);
        var writer = new ArrayBufferWriter<char>();

        JsonConverter.UTF16.Serialize(model, writer);

        Assert.AreEqual(expected, new string(writer.WrittenSpan));
    }

    [Test]
    public void Utf16BinaryWriterTest()
    {
        var model = CreateModel();
        var json = JsonConverter.UTF16.Serialize(model);

        using var defaultStream = new MemoryStream();
        using (var defaultWriter = new BinaryWriter(defaultStream, Encoding.UTF8, true))
        {
            JsonConverter.UTF16.Serialize(model, defaultWriter);
        }

        // BinaryWriter.Write(ReadOnlySpan<char>) encodes with the writer's Encoding, so the default one stores UTF-8
        Assert.AreEqual(Encoding.UTF8.GetBytes(json), defaultStream.ToArray());

        using var unicodeStream = new MemoryStream();
        using (var unicodeWriter = new BinaryWriter(unicodeStream, Encoding.Unicode, true))
        {
            JsonConverter.UTF16.Serialize(model, unicodeWriter);
        }

        var unicode = unicodeStream.ToArray();
        Assert.AreEqual(Encoding.Unicode.GetBytes(json), unicode);
        Assert.IsFalse(unicode.Length >= 2 && unicode[0] == 0xFF && unicode[1] == 0xFE, "no byte order mark may be written");
    }

    [Test]
    public void Utf8StreamSyncTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF8.Serialize(model);
        using var stream = new MemoryStream();

        JsonConverter.UTF8.Serialize(model, stream);

        var bytes = stream.ToArray();
        Assert.AreEqual(expected, bytes);
        Assert.IsFalse(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF, "no byte order mark may be written");
    }

    [Test]
    public async Task Utf8StreamAsyncTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF8.Serialize(model);
        using var stream = new AsyncMemoryStream();

        await JsonConverter.UTF8.SerializeAsync(model, stream);

        Assert.AreEqual(expected, stream.ToArray());
    }

    [Test]
    public async Task Utf8StreamAsyncCancellationTest()
    {
        var model = CreateModel();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        using var stream = new CancelingStream();

        Assert.CatchAsync<OperationCanceledException>(async () => await JsonConverter.UTF8.SerializeAsync(model, stream, cts.Token));
        Assert.AreEqual(0, stream.Length);
    }

    [Test]
    public async Task Utf16TextWriterAsyncCancellationTest()
    {
        var model = CreateModel();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        using var writer = new CancelingWriter();

        Assert.CatchAsync<OperationCanceledException>(async () => await JsonConverter.UTF16.SerializeAsync(model, writer, cts.Token));
        Assert.AreEqual(string.Empty, writer.ToString());
    }

    [Test]
    public void Utf8BufferWriterSyncTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF8.Serialize(model);
        var writer = new ArrayBufferWriter<byte>();

        JsonConverter.UTF8.Serialize(model, writer);

        Assert.AreEqual(expected, writer.WrittenSpan.ToArray());
    }

    [Test]
    public void Utf8BinaryWriterTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF8.Serialize(model);
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.Unicode, true))
        {
            // the UTF-8 entry writes raw bytes, so the writer's own Encoding must not be involved
            JsonConverter.UTF8.Serialize(model, writer);
        }

        Assert.AreEqual(expected, stream.ToArray());
    }

    [Test]
    public void PooledBufferReturnedWhenTargetThrowsTest()
    {
        var model = CreateModel();
        var expected = JsonConverter.UTF16.Serialize(model);
        using var throwingWriter = new ThrowingWriter();

        Assert.Throws<InvalidOperationException>(() => JsonConverter.UTF16.Serialize(model, throwingWriter));

        // the rented buffer must be back in the pool, so the next document is still serialized correctly
        Assert.AreEqual(expected, JsonConverter.UTF16.Serialize(model));

        var utf8 = JsonConverter.UTF8.Serialize(model);
        using var throwingStream = new ThrowingStream();
        Assert.Throws<InvalidOperationException>(() => JsonConverter.UTF8.Serialize(model, throwingStream));
        Assert.AreEqual(utf8, JsonConverter.UTF8.Serialize(model));
    }

    [Test]
    public void WhitespacePrefixedDocumentTest()
    {
        var model = CreateModel();
        var json = JsonConverter.UTF16.Serialize(model);
        var utf8 = Encoding.UTF8.GetBytes("\r\n\t " + json);

        Assert.AreEqual(model.id, JsonConverter.UTF16.Deserialize<OutputTargetModel>("\r\n\t " + json).id);
        Assert.AreEqual(model.id, JsonConverter.UTF16.Deserialize<OutputTargetModel>(" \r\n\t" + json).id);
        Assert.AreEqual(model.id, JsonConverter.UTF8.Deserialize<OutputTargetModel>(utf8).id);
    }
}
