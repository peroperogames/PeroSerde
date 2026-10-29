/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 */

using System;
using System.Text;
using NUnit.Framework;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    [SerdeJsonSerializable]
    public partial class CursorKeySimpleObject
    {
        public long nameabcd;
    }

    [SerdeJsonSerializable]
    public partial class CursorKeyCollisionObject
    {
        public long nameabcd1;
        public long nameabcd2;
    }

    /// <summary>
    /// key 前缀匹配的等价性与回退路径测试。
    /// 生成期前缀常量与运行期打包只要有一位不一致，成员就会静默保持默认值，因此必须有测试锁住。
    /// </summary>
    public sealed unsafe class CursorKeyTests
    {
        private static ulong NaivePrefix(ReadOnlySpan<byte> key)
        {
            ulong value = 0;
            for (var i = 0; i < key.Length && i < 8; i++)
            {
                value |= (ulong) key[i] << (i * 8);
            }

            return value;
        }

        private static ulong RuntimePrefix(ReadOnlySpan<byte> key)
        {
            fixed (byte* p = key)
            {
                return StringHelper.GetKeyPrefix(p, key.Length);
            }
        }
        [Test]
        public void KeyPrefixMatchesNaiveTest()
        {
            foreach (var length in new[] { 0, 1, 2, 7, 8, 9, 15, 16, 17 })
            {
                var buffer = new byte[length];
                for (var i = 0; i < length; i++)
                {
                    buffer[i] = (byte) (0x20 + (i * 7) % 0x5F);
                }

                Assert.AreEqual(NaivePrefix(buffer), RuntimePrefix(buffer), $"length={length}");
            }

            var special = new byte[] { 0x00, 0x80, 0xFF, 0x7F, 0x00, 0x41, 0x42, 0x43, 0x44 };
            Assert.AreEqual(NaivePrefix(special), RuntimePrefix(special));
        }

        [Test]
        public void EightByteMemberNameTest()
        {
            var json = Encoding.UTF8.GetBytes("{\"nameabcd\":123456789}");
            var value = JsonConverter.UTF8.Deserialize<CursorKeySimpleObject>(json);
            Assert.AreEqual(123456789L, value.nameabcd);
        }

        [Test]
        public void SharedPrefixMembersTest()
        {
            // 两个成员前 8 字符相同 -> 前缀冲突 -> 整体回退旧路径，但仍必须正确反序列化
            var json = Encoding.UTF8.GetBytes("{\"nameabcd1\":11,\"nameabcd2\":22}");
            var value = JsonConverter.UTF8.Deserialize<CursorKeyCollisionObject>(json);
            Assert.AreEqual(11L, value.nameabcd1);
            Assert.AreEqual(22L, value.nameabcd2);
        }
    }
}
