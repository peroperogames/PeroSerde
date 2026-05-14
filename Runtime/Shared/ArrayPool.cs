using System.Buffers;

namespace PeroTools2.Serde
{
    internal static class ArrayPool
    {
        internal static readonly ArrayPool<char> UTF16 = ArrayPool<char>.Create(int.MaxValue, 8);
        internal static readonly ArrayPool<byte> UTF8 = ArrayPool<byte>.Create(int.MaxValue, 8);
    }
}