/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/12
 * Description: GuidBits
 */

using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde
{
    /// <summary>
    /// Guid 位信息
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct GuidBits
    {
        [FieldOffset(0)]
        public readonly Guid value;

        [FieldOffset(0)]
        public readonly byte a4;

        [FieldOffset(1)]
        public readonly byte a3;

        [FieldOffset(2)]
        public readonly byte a2;

        [FieldOffset(3)]
        public readonly byte a1;

        // short _b
        [FieldOffset(4)]
        public readonly byte b2;

        [FieldOffset(5)]
        public readonly byte b1;

        // short _c
        [FieldOffset(6)]
        public readonly byte c2;

        [FieldOffset(7)]
        public readonly byte c1;

        // 往下都是单独的byte
        [FieldOffset(8)]
        public readonly byte d;

        [FieldOffset(9)]
        public readonly byte e;

        [FieldOffset(10)]
        public readonly byte f;

        [FieldOffset(11)]
        public readonly byte g;

        [FieldOffset(12)]
        public readonly byte h;

        [FieldOffset(13)]
        public readonly byte i;

        [FieldOffset(14)]
        public readonly byte j;

        [FieldOffset(15)]
        public readonly byte k;

        // Enumerable.Range(0, 256).Select(x => (int)BitConverter.ToString(new byte[] { (byte)x }).ToLower()[0])
        // 48代表0的ASCII码, 以此类推
        private static readonly byte[] byteToHexStringHigh = new byte[256]
        {
            48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50,
            50, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53,
            53, 53, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56,
            56, 56, 56, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98,
            98, 98, 98, 98, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 101, 101, 101, 101, 101,
            101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102
        };

        // Enumerable.Range(0, 256).Select(x => (int)BitConverter.ToString(new byte[] { (byte)x }).ToLower()[1])
        private static readonly byte[] byteToHexStringLow = new byte[256]
        {
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99,
            100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57,
            97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54,
            55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51,
            52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48,
            49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102
        };

        public GuidBits(ref Guid value) // 使用ref减少一次复制
        {
            this = default; // 为所有字段赋初值
            this.value = value;
        }

        public unsafe void WriteUtf8(byte* dest)
        {
            // 多字节的数字(short, int, long)需要区分端序
            if (BitConverter.IsLittleEndian) // 小端序数字转小端序char[]
            {
                // int(_a)
                dest[0] = byteToHexStringHigh[a1];
                dest[1] = byteToHexStringLow[a1];
                dest[2] = byteToHexStringHigh[a2];
                dest[3] = byteToHexStringLow[a2];
                dest[4] = byteToHexStringHigh[a3];
                dest[5] = byteToHexStringLow[a3];
                dest[6] = byteToHexStringHigh[a4];
                dest[7] = byteToHexStringLow[a4];
                dest[8] = (byte) '-';
                // short(_b)
                dest[9] = byteToHexStringHigh[b1];
                dest[10] = byteToHexStringLow[b1];
                dest[11] = byteToHexStringHigh[b2];
                dest[12] = byteToHexStringLow[b2];
                dest[13] = (byte) '-';
                // short(_c)
                dest[14] = byteToHexStringHigh[c1];
                dest[15] = byteToHexStringLow[c1];
                dest[16] = byteToHexStringHigh[c2];
                dest[17] = byteToHexStringLow[c2];
            }
            else
            {
                // int(_a)
                dest[0] = byteToHexStringHigh[a4];
                dest[1] = byteToHexStringLow[a4];
                dest[2] = byteToHexStringHigh[a3];
                dest[3] = byteToHexStringLow[a3];
                dest[4] = byteToHexStringHigh[a2];
                dest[5] = byteToHexStringLow[a2];
                dest[6] = byteToHexStringHigh[a1];
                dest[7] = byteToHexStringLow[a1];
                dest[8] = (byte) '-';
                // short(_b)
                dest[9] = byteToHexStringHigh[b2];
                dest[10] = byteToHexStringLow[b2];
                dest[11] = byteToHexStringHigh[b1];
                dest[12] = byteToHexStringLow[b1];
                dest[13] = (byte) '-';
                // short(_c)
                dest[14] = byteToHexStringHigh[c2];
                dest[15] = byteToHexStringLow[c2];
                dest[16] = byteToHexStringHigh[c1];
                dest[17] = byteToHexStringLow[c1];
            }
            // 单个byte不用担心端序的问题
            dest[18] = (byte) '-';
            dest[19] = byteToHexStringHigh[d];
            dest[20] = byteToHexStringLow[d];
            dest[21] = byteToHexStringHigh[e];
            dest[22] = byteToHexStringLow[e];
            dest[23] = (byte) '-';
            dest[24] = byteToHexStringHigh[f];
            dest[25] = byteToHexStringLow[f];
            dest[26] = byteToHexStringHigh[g];
            dest[27] = byteToHexStringLow[g];
            dest[28] = byteToHexStringHigh[h];
            dest[29] = byteToHexStringLow[h];
            dest[30] = byteToHexStringHigh[i];
            dest[31] = byteToHexStringLow[i];
            dest[32] = byteToHexStringHigh[j];
            dest[33] = byteToHexStringLow[j];
            dest[34] = byteToHexStringHigh[k];
            dest[35] = byteToHexStringLow[k];
        }

        public unsafe void WriteUtf16(char* dest)
        {
            // 多字节的数字(short, int, long)需要区分端序
            if (BitConverter.IsLittleEndian) // 小端序数字转小端序char[]
            {
                // int(_a)
                dest[0] = (char) byteToHexStringHigh[a1];
                dest[1] = (char) byteToHexStringLow[a1];
                dest[2] = (char) byteToHexStringHigh[a2];
                dest[3] = (char) byteToHexStringLow[a2];
                dest[4] = (char) byteToHexStringHigh[a3];
                dest[5] = (char) byteToHexStringLow[a3];
                dest[6] = (char) byteToHexStringHigh[a4];
                dest[7] = (char) byteToHexStringLow[a4];
                dest[8] = '-';
                // short(_b)
                dest[9] = (char) byteToHexStringHigh[b1];
                dest[10] = (char) byteToHexStringLow[b1];
                dest[11] = (char) byteToHexStringHigh[b2];
                dest[12] = (char) byteToHexStringLow[b2];
                dest[13] = '-';
                // short(_c)
                dest[14] = (char) byteToHexStringHigh[c1];
                dest[15] = (char) byteToHexStringLow[c1];
                dest[16] = (char) byteToHexStringHigh[c2];
                dest[17] = (char) byteToHexStringLow[c2];
            }
            else
            {
                // int(_a)
                dest[0] = (char) byteToHexStringHigh[a4];
                dest[1] = (char) byteToHexStringLow[a4];
                dest[2] = (char) byteToHexStringHigh[a3];
                dest[3] = (char) byteToHexStringLow[a3];
                dest[4] = (char) byteToHexStringHigh[a2];
                dest[5] = (char) byteToHexStringLow[a2];
                dest[6] = (char) byteToHexStringHigh[a1];
                dest[7] = (char) byteToHexStringLow[a1];
                dest[8] = '-';
                // short(_b)
                dest[9] = (char) byteToHexStringHigh[b2];
                dest[10] = (char) byteToHexStringLow[b2];
                dest[11] = (char) byteToHexStringHigh[b1];
                dest[12] = (char) byteToHexStringLow[b1];
                dest[13] = '-';
                // short(_c)
                dest[14] = (char) byteToHexStringHigh[c2];
                dest[15] = (char) byteToHexStringLow[c2];
                dest[16] = (char) byteToHexStringHigh[c1];
                dest[17] = (char) byteToHexStringLow[c1];
            }
            // 单个byte不用担心端序的问题
            dest[18] = '-';
            dest[19] = (char) byteToHexStringHigh[d];
            dest[20] = (char) byteToHexStringLow[d];
            dest[21] = (char) byteToHexStringHigh[e];
            dest[22] = (char) byteToHexStringLow[e];
            dest[23] = '-';
            dest[24] = (char) byteToHexStringHigh[f];
            dest[25] = (char) byteToHexStringLow[f];
            dest[26] = (char) byteToHexStringHigh[g];
            dest[27] = (char) byteToHexStringLow[g];
            dest[28] = (char) byteToHexStringHigh[h];
            dest[29] = (char) byteToHexStringLow[h];
            dest[30] = (char) byteToHexStringHigh[i];
            dest[31] = (char) byteToHexStringLow[i];
            dest[32] = (char) byteToHexStringHigh[j];
            dest[33] = (char) byteToHexStringLow[j];
            dest[34] = (char) byteToHexStringHigh[k];
            dest[35] = (char) byteToHexStringLow[k];
        }
    }
}