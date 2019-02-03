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

        public bool TryAddMultiThread(in IdentifierNumberPair pair)
        {
            int last;
            do
            {
                last = Length;
                if (last == Capacity)
                    return false;
            } while (last != Interlocked.CompareExchange(ref Length, last + 1, last));
            Values[last] = pair;
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