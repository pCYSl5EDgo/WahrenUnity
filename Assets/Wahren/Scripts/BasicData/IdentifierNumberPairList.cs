using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct IdentifierNumberPairList
    {
        public IdentifierNumberPair* Values;
        public int Capacity;
        public int Length;

        public IdentifierNumberPairList(int capacity, Allocator allocator)
        {
            Length = 0;
            Capacity = capacity;
            if (Capacity == 0)
                Values = null;
            else
                Values = (IdentifierNumberPair*)UnsafeUtility.Malloc(sizeof(IdentifierNumberPair) * capacity, 4, allocator);
        }

        public override string ToString() => "capacity : " + Capacity + "\nlength : " + Length;

        public void Lengthen(Allocator allocator)
        {
            if (Capacity == 0) return;
            int size = sizeof(IdentifierNumberPair) * Capacity;
            var _ = UnsafeUtility.Malloc(size * 2, 4, allocator);
            UnsafeUtility.MemCpy(_, Values, size);
            UnsafeUtility.Free(Values, allocator);
            Values = (IdentifierNumberPair*)_;
            Capacity *= 2;
        }
        public void Dispose(Allocator allocator)
        {
            if (Capacity != 0)
            {
                UnsafeUtility.Free(Values, allocator);
                this = default;
            }
        }
    }
}