using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
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

        public void Lengthen(Allocator allocator = Allocator.Persistent)
        {
            if (Capacity == 0) return;
            int size = sizeof(IdentifierNumberPair) * Capacity;
            var _ = UnsafeUtility.Malloc(size * 2, 4, allocator);
            UnsafeUtility.MemCpy(_, Values, size);
            UnsafeUtility.Free(Values, allocator);
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

        public static IdentifierNumberPairList MallocTemp(int capacity)
        => new IdentifierNumberPairList
        {
            Capacity = capacity,
            Length = 0,
            Values = (IdentifierNumberPair*)UnsafeUtility.Malloc(sizeof(IdentifierNumberPair) * capacity, 4, Allocator.Temp),
        };
        public static void FreeTemp(ref IdentifierNumberPairList list)
        {
            if (list.Capacity != 0)
            {
                UnsafeUtility.Free(list.Values, Allocator.Temp);
            }
            list = default;
        }
    }
}