/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/27-18:49:38
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 数组的 Json 格式化器
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [PublicAPI]
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public sealed class ArrayFormatter<T> : ISerdeJsonFormatter<T[]>
    {
        private static readonly IFixedSerdeJsonFormatter<T> s_FixedFormatter;
        private static readonly ISerdeJsonFormatter<T> s_UnfixedFormatter;
        private static readonly bool s_IsFixed;

        static ArrayFormatter()
        {
            var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
            if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
            {
                s_IsFixed = true;
                s_FixedFormatter = fixedFormatter;
            }
            else if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
            {
                s_UnfixedFormatter = unfixedFormatter;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(T[] value, ref JsonWriterUtf8 writer)
        {
            if (s_IsFixed)
            {
                writer.WriteCollection(value, s_FixedFormatter);
            }
            else
            {
                writer.WriteCollection(value, s_UnfixedFormatter);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(T[] value, ref JsonWriterUtf16 writer)
        {
            if (s_IsFixed)
            {
                writer.WriteCollection(value, s_FixedFormatter);
            }
            else
            {
                writer.WriteCollection(value, s_UnfixedFormatter);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var list = ListPool<T>.shared.Rent();
            try
            {
                if (s_IsFixed)
                {
                    reader.ReadCollection(list, s_FixedFormatter, false, out _);
                }
                else
                {
                    reader.ReadCollection(list, s_UnfixedFormatter, false, out _);
                }

                return list?.ToArray();
            }
            finally
            {
                ListPool<T>.shared.Return(list);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            var list = ListPool<T>.shared.Rent();
            try
            {
                if (s_IsFixed)
                {
                    reader.ReadCollection(list, s_FixedFormatter, false, out _);
                }
                else
                {
                    reader.ReadCollection(list, s_UnfixedFormatter, false, out _);
                }

                return list?.ToArray();
            }
            finally
            {
                ListPool<T>.shared.Return(list);
            }
        }
    }
}