using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public unsafe struct IdentifierNumberPairList : IDisposable
    {
        public int Capacity;
        public int Length;
        public IdentifierNumberPair* Values;

        public IdentifierNumberPairList(int capacity)
        {
            Length = 0;
            Capacity = capacity;
            if (Capacity == 0)
                Values = null;
            else
                Values = (IdentifierNumberPair*)UnsafeUtility.Malloc(sizeof(IdentifierNumberPair) * capacity, 4, Allocator.Persistent);
        }

        public bool TryAddMultiThread(IdentifierNumberPair* pairs, int length, out int start)
        {
            do
            {
                start = Length;
                if (start + length > Capacity)
                    return false;
            } while (start != Interlocked.CompareExchange(ref Length, start + length, start));
            UnsafeUtility.MemCpy(Values + start, pairs, length * sizeof(IdentifierNumberPair));
            return true;
        }

        public void Lengthen()
        {
            if (Capacity == 0) return;
            int size = sizeof(IdentifierNumberPair) * Capacity;
            var _ = UnsafeUtility.Malloc(size * 2, 4, Allocator.Persistent);
            UnsafeUtility.MemCpy(_, Values, size);
            UnsafeUtility.Free(Values, Allocator.Persistent);
            Values = (IdentifierNumberPair*)_;
            Capacity *= 2;
        }
        public void Dispose()
        {
            if (Capacity != 0)
            {
                UnsafeUtility.Free(Values, Allocator.Persistent);
                this = default;
            }
        }
    }
}