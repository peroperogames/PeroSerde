/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/06
 */

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    [SerdeJsonSerializable]
    public partial class OutputTargetModel
    {
        public int id;
        public string label;
    }

    /// <summary>
    /// Tests for the <see cref="JsonConverter"/> overloads that write into a caller supplied target
    /// (TextWriter / Stream / IBufferWriter / BinaryWriter, synchronous and asynchronous).
    /// This file is compiled both by the Unity edit mode test assembly and by the .NET test project.
    /// </summary>
    public sealed class JsonConverterOutputTests
    {
        private const string ExpectedDocument = "{\"id\":7,\"label\":\"七\"}";
        private const int LargeLabelLength = 400000;

        private static OutputTargetModel CreateModel() => new OutputTargetModel { id = 7, label = "七" };

        private static OutputTargetModel CreateLargeModel() =>
            new OutputTargetModel { id = 7, label = new string('字', LargeLabelLength) };

        /// <summary>Reads the writer pool queue so buffer leaks become observable</summary>
        private static ConcurrentQueue<ReusableBufferWriter<T>> GetPoolQueue<T>() where T : unmanaged
        {
            var field = typeof(ReusableBufferWriter<T>.Pool).GetField("s_Queue", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(field, "the writer pool queue field is no longer named s_Queue");
            return (ConcurrentQueue<ReusableBufferWriter<T>>)field.GetValue(null);
        }

        private static async Task AssertThrowsAsync<TException>(Func<Task> action) where TException : Exception
        {
            // Unity ships NUnit 3.5, which has no Assert.CatchAsync / Assert.ThrowsAsync
            try
            {
                await action();
            }
            catch (TException)
            {
                return;
            }
            Assert.Fail("expected " + typeof(TException).Name);
        }

        private sealed class CharBufferWriter : IBufferWriter<char>
        {
            private char[] m_Buffer = new char[16];
            private int m_Written;

            internal ReadOnlySpan<char> WrittenSpan => new ReadOnlySpan<char>(m_Buffer, 0, m_Written);

            public void Advance(int count) => m_Written += count;

            public Memory<char> GetMemory(int sizeHint = 0)
            {
                EnsureCapacity(sizeHint);
                return new Memory<char>(m_Buffer, m_Written, m_Buffer.Length - m_Written);
            }

            public Span<char> GetSpan(int sizeHint = 0)
            {
                EnsureCapacity(sizeHint);
                return new Span<char>(m_Buffer, m_Written, m_Buffer.Length - m_Written);
            }

            private int EnsureCapacity(int sizeHint)
            {
                // the IBufferWriter contract wants a non empty span for a zero size hint
                if (sizeHint < 1)
                {
                    sizeHint = 1;
                }
                if (sizeHint > m_Buffer.Length - m_Written)
                {
                    Array.Resize(ref m_Buffer, m_Written + Math.Max(sizeHint, m_Buffer.Length));
                }
                return m_Written;
            }
        }

        private sealed class ByteBufferWriter : IBufferWriter<byte>
        {
            private byte[] m_Buffer = new byte[16];
            private int m_Written;

            internal ReadOnlySpan<byte> WrittenSpan => new ReadOnlySpan<byte>(m_Buffer, 0, m_Written);

            public void Advance(int count) => m_Written += count;

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                EnsureCapacity(sizeHint);
                return new Memory<byte>(m_Buffer, m_Written, m_Buffer.Length - m_Written);
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                EnsureCapacity(sizeHint);
                return new Span<byte>(m_Buffer, m_Written, m_Buffer.Length - m_Written);
            }

            private int EnsureCapacity(int sizeHint)
            {
                // the IBufferWriter contract wants a non empty span for a zero size hint
                if (sizeHint < 1)
                {
                    sizeHint = 1;
                }
                if (sizeHint > m_Buffer.Length - m_Written)
                {
                    Array.Resize(ref m_Buffer, m_Written + Math.Max(sizeHint, m_Buffer.Length));
                }
                return m_Written;
            }
        }

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

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
                WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
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

            public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default) =>
                throw new InvalidOperationException("write failed");
        }

        private sealed class ThrowingStream : MemoryStream
        {
            public override void Write(ReadOnlySpan<byte> buffer) => throw new InvalidOperationException("write failed");

            public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
                throw new InvalidOperationException("write failed");
        }

        [Test]
        public void LiteralDocumentTest()
        {
            var model = CreateModel();

            // the expectation is a literal, so a broken writer cannot agree with itself and pass anyway
            Assert.AreEqual(ExpectedDocument, JsonConverter.UTF16.Serialize(model));

            using (var writer = new StringWriter())
            {
                JsonConverter.UTF16.Serialize(model, writer);
                Assert.AreEqual(ExpectedDocument, writer.ToString());
            }

            using (var stream = new MemoryStream())
            {
                JsonConverter.UTF8.Serialize(model, stream);
                Assert.AreEqual(Encoding.UTF8.GetBytes(ExpectedDocument), stream.ToArray());
            }
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
            var writer = new CharBufferWriter();

            JsonConverter.UTF16.Serialize(model, writer);

            Assert.AreEqual(expected, new string(writer.WrittenSpan));
        }

        [Test]
        public void Utf8BufferWriterSyncTest()
        {
            var model = CreateModel();
            var expected = JsonConverter.UTF8.Serialize(model);
            var writer = new ByteBufferWriter();

            JsonConverter.UTF8.Serialize(model, writer);

            Assert.AreEqual(expected, writer.WrittenSpan.ToArray());
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
        public void LargeDocumentTargetsTest()
        {
            var model = CreateLargeModel();
            var json = JsonConverter.UTF16.Serialize(model);
            var bytes = JsonConverter.UTF8.Serialize(model);

            // larger than the initial 256KB segment, so the writer has to grow and rebuild its buffer
            Assert.Greater(json.Length, 262144);

            using (var writer = new StringWriter())
            {
                JsonConverter.UTF16.Serialize(model, writer);
                Assert.AreEqual(json, writer.ToString());
            }

            using (var stream = new MemoryStream())
            {
                JsonConverter.UTF8.Serialize(model, stream);
                Assert.AreEqual(bytes, stream.ToArray());
            }

            var charWriter = new CharBufferWriter();
            JsonConverter.UTF16.Serialize(model, charWriter);
            Assert.AreEqual(json, new string(charWriter.WrittenSpan));

            var byteWriter = new ByteBufferWriter();
            JsonConverter.UTF8.Serialize(model, byteWriter);
            Assert.AreEqual(bytes, byteWriter.WrittenSpan.ToArray());

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.Unicode, true))
                {
                    JsonConverter.UTF16.Serialize(model, writer);
                }
                Assert.AreEqual(Encoding.Unicode.GetBytes(json), stream.ToArray());
            }
        }

        [Test]
        public async Task LargeDocumentAsyncTargetsTest()
        {
            var model = CreateLargeModel();
            var json = JsonConverter.UTF16.Serialize(model);
            var bytes = JsonConverter.UTF8.Serialize(model);

            using (var writer = new AsyncStringWriter())
            {
                await JsonConverter.UTF16.SerializeAsync(model, writer);
                Assert.AreEqual(json, writer.ToString());
            }

            using (var stream = new AsyncMemoryStream())
            {
                await JsonConverter.UTF8.SerializeAsync(model, stream);
                Assert.AreEqual(bytes, stream.ToArray());
            }
        }

        [Test]
        public void PooledBufferReturnedWhenTargetThrowsTest()
        {
            var model = CreateModel();
            var charQueue = GetPoolQueue<char>();
            // put one writer in the pool first, otherwise the very first rent creates a new one and the count grows
            ReusableBufferWriter<char>.Pool.Return(ReusableBufferWriter<char>.Pool.Rent());
            var charCount = charQueue.Count;

            using (var throwingWriter = new ThrowingWriter())
            {
                Assert.Throws<InvalidOperationException>(() => JsonConverter.UTF16.Serialize(model, throwingWriter));
            }

            Assert.AreEqual(charCount, charQueue.Count, "the rented writer has to be returned to the pool when the target throws");

            var byteQueue = GetPoolQueue<byte>();
            ReusableBufferWriter<byte>.Pool.Return(ReusableBufferWriter<byte>.Pool.Rent());
            var byteCount = byteQueue.Count;

            using (var throwingStream = new ThrowingStream())
            {
                Assert.Throws<InvalidOperationException>(() => JsonConverter.UTF8.Serialize(model, throwingStream));
            }

            Assert.AreEqual(byteCount, byteQueue.Count, "the rented writer has to be returned to the pool when the target throws");
        }

        [Test]
        public async Task PooledBufferReturnedWhenAsyncTargetThrowsTest()
        {
            var model = CreateModel();
            var charQueue = GetPoolQueue<char>();
            // put one writer in the pool first, otherwise the very first rent creates a new one and the count grows
            ReusableBufferWriter<char>.Pool.Return(ReusableBufferWriter<char>.Pool.Rent());
            var charCount = charQueue.Count;

            using (var throwingWriter = new ThrowingWriter())
            {
                await AssertThrowsAsync<InvalidOperationException>(async () => await JsonConverter.UTF16.SerializeAsync(model, throwingWriter));
            }

            Assert.AreEqual(charCount, charQueue.Count, "the rented writer has to be returned to the pool when the asynchronous write fails");

            var byteQueue = GetPoolQueue<byte>();
            ReusableBufferWriter<byte>.Pool.Return(ReusableBufferWriter<byte>.Pool.Rent());
            var byteCount = byteQueue.Count;

            using (var throwingStream = new ThrowingStream())
            {
                await AssertThrowsAsync<InvalidOperationException>(async () => await JsonConverter.UTF8.SerializeAsync(model, throwingStream));
            }

            Assert.AreEqual(byteCount, byteQueue.Count, "the rented writer has to be returned to the pool when the asynchronous write fails");
        }

        [Test]
        public async Task Utf8StreamAsyncCancellationTest()
        {
            var model = CreateModel();
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            using var stream = new CancelingStream();

            await AssertThrowsAsync<OperationCanceledException>(async () => await JsonConverter.UTF8.SerializeAsync(model, stream, cts.Token));
            Assert.AreEqual(0, stream.Length);
        }

        [Test]
        public async Task Utf16TextWriterAsyncCancellationTest()
        {
            var model = CreateModel();
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            using var writer = new CancelingWriter();

            await AssertThrowsAsync<OperationCanceledException>(async () => await JsonConverter.UTF16.SerializeAsync(model, writer, cts.Token));
            Assert.AreEqual(string.Empty, writer.ToString());
        }

        [Test]
        public void WhitespaceOnlyDocumentTest()
        {
            // a document made of nothing but whitespace has to be treated as empty, which the reader alone does not do
            Assert.AreEqual(0, JsonConverter.UTF16.Deserialize<int>("\r\n\t "));
            Assert.AreEqual(0, JsonConverter.UTF8.Deserialize<int>(Encoding.UTF8.GetBytes("\r\n\t ")));
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
}
