/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 * Description: MemoryPackWriter.Unmanaged
 */

using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.MemoryPack
{
    public ref partial struct MemoryPackWriter<TBufferWriter>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1>(in T1 v1)
            where T1 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>();
            ref var spanRef = ref GetSpanReference(size);
            Unsafe.WriteUnaligned(ref spanRef, v1);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2>(in T1 v1, in T2 v2)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3>(in T1 v1, in T2 v2, in T3 v3)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4>(in T1 v1, in T2 v2, in T3 v3, in T4 v4)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
            where T9 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
            where T9 : unmanaged
            where T10 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
            where T9 : unmanaged
            where T10 : unmanaged
            where T11 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
            where T9 : unmanaged
            where T10 : unmanaged
            where T11 : unmanaged
            where T12 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12, in T13 v13)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
            where T9 : unmanaged
            where T10 : unmanaged
            where T11 : unmanaged
            where T12 : unmanaged
            where T13 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>() + Unsafe.SizeOf<T13>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            offset += Unsafe.SizeOf<T12>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v13);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12, in T13 v13, in T14 v14)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
            where T9 : unmanaged
            where T10 : unmanaged
            where T11 : unmanaged
            where T12 : unmanaged
            where T13 : unmanaged
            where T14 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>() + Unsafe.SizeOf<T13>() + Unsafe.SizeOf<T14>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            offset += Unsafe.SizeOf<T12>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v13);
            offset += Unsafe.SizeOf<T13>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v14);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12, in T13 v13, in T14 v14, in T15 v15)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
            where T9 : unmanaged
            where T10 : unmanaged
            where T11 : unmanaged
            where T12 : unmanaged
            where T13 : unmanaged
            where T14 : unmanaged
            where T15 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>() + Unsafe.SizeOf<T13>() + Unsafe.SizeOf<T14>() + Unsafe.SizeOf<T15>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            offset += Unsafe.SizeOf<T12>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v13);
            offset += Unsafe.SizeOf<T13>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v14);
            offset += Unsafe.SizeOf<T14>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v15);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12, in T13 v13, in T14 v14, in T15 v15, in T16 v16)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged
            where T9 : unmanaged
            where T10 : unmanaged
            where T11 : unmanaged
            where T12 : unmanaged
            where T13 : unmanaged
            where T14 : unmanaged
            where T15 : unmanaged
            where T16 : unmanaged
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>() + Unsafe.SizeOf<T13>() + Unsafe.SizeOf<T14>() + Unsafe.SizeOf<T15>() + Unsafe.SizeOf<T16>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            offset += Unsafe.SizeOf<T12>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v13);
            offset += Unsafe.SizeOf<T13>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v14);
            offset += Unsafe.SizeOf<T14>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v15);
            offset += Unsafe.SizeOf<T15>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v16);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1>(in T1 v1)
        {
            var size = Unsafe.SizeOf<T1>();
            ref var spanRef = ref GetSpanReference(size);
            Unsafe.WriteUnaligned(ref spanRef, v1);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2>(in T1 v1, in T2 v2)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3>(in T1 v1, in T2 v2, in T3 v3)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4>(in T1 v1, in T2 v2, in T3 v3, in T4 v4)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12, in T13 v13)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>() + Unsafe.SizeOf<T13>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            offset += Unsafe.SizeOf<T12>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v13);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12, in T13 v13, in T14 v14)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>() + Unsafe.SizeOf<T13>() + Unsafe.SizeOf<T14>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            offset += Unsafe.SizeOf<T12>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v13);
            offset += Unsafe.SizeOf<T13>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v14);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12, in T13 v13, in T14 v14, in T15 v15)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>() + Unsafe.SizeOf<T13>() + Unsafe.SizeOf<T14>() + Unsafe.SizeOf<T15>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            offset += Unsafe.SizeOf<T12>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v13);
            offset += Unsafe.SizeOf<T13>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v14);
            offset += Unsafe.SizeOf<T14>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v15);
            Advance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(in T1 v1, in T2 v2, in T3 v3, in T4 v4, in T5 v5, in T6 v6, in T7 v7, in T8 v8, in T9 v9, in T10 v10, in T11 v11, in T12 v12, in T13 v13, in T14 v14, in T15 v15, in T16 v16)
        {
            var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>() + Unsafe.SizeOf<T4>() + Unsafe.SizeOf<T5>() + Unsafe.SizeOf<T6>() + Unsafe.SizeOf<T7>() + Unsafe.SizeOf<T8>() + Unsafe.SizeOf<T9>() + Unsafe.SizeOf<T10>() + Unsafe.SizeOf<T11>() + Unsafe.SizeOf<T12>() + Unsafe.SizeOf<T13>() + Unsafe.SizeOf<T14>() + Unsafe.SizeOf<T15>() + Unsafe.SizeOf<T16>();
            ref var spanRef = ref GetSpanReference(size);
            var offset = 0;
            Unsafe.WriteUnaligned(ref spanRef, v1);
            offset += Unsafe.SizeOf<T1>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v2);
            offset += Unsafe.SizeOf<T2>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v3);
            offset += Unsafe.SizeOf<T3>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v4);
            offset += Unsafe.SizeOf<T4>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v5);
            offset += Unsafe.SizeOf<T5>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v6);
            offset += Unsafe.SizeOf<T6>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v7);
            offset += Unsafe.SizeOf<T7>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v8);
            offset += Unsafe.SizeOf<T8>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v9);
            offset += Unsafe.SizeOf<T9>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v10);
            offset += Unsafe.SizeOf<T10>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v11);
            offset += Unsafe.SizeOf<T11>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v12);
            offset += Unsafe.SizeOf<T12>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v13);
            offset += Unsafe.SizeOf<T13>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v14);
            offset += Unsafe.SizeOf<T14>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v15);
            offset += Unsafe.SizeOf<T15>();
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, offset), v16);
            Advance(size);
        }

    }
}
