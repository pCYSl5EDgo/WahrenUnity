using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct ASTValueTypePairList : IDisposable
    {
        public int Capacity;
        public int Length;
        public ASTValueTypePair* Values;

        public ASTValueTypePairList(int capacity)
        {
            if (capacity == 0)
            {
                this = default;
                return;
            }
            Capacity = capacity;
            Length = 0;
            Values = (ASTValueTypePair*)UnsafeUtility.Malloc(sizeof(ASTValueTypePair) * capacity, 4, Allocator.Persistent);
        }
        public (int start, int length) TryAddBulkMultiThread(in ASTValueTypePairList list)
        {
            if (list.Length == 0)
                return (0, 0);
            (int start, int length) p = (-1, list.Length);
            do
            {
                p.start = Length;
                if (p.start + p.length > Capacity)
                    return (-1, 0);
            } while (p.start != Interlocked.CompareExchange(ref Length, p.start + p.length, p.start));
            UnsafeUtility.MemCpy(Values + p.start, list.Values, sizeof(ASTValueTypePair) * p.length);
            return p;
        }

        public void Dispose()
        {
            if (Capacity != 0)
                UnsafeUtility.Free(Values, Allocator.Persistent);
            this = default;
        }

        public static ASTValueTypePairList MallocTemp(int capacity)
        => new ASTValueTypePairList
        {
            Capacity = capacity,
            Length = 0,
            Values = (ASTValueTypePair*)UnsafeUtility.Malloc(sizeof(ASTValueTypePair) * capacity, 4, Allocator.Temp),
        };
        public static void FreeTemp(ref ASTValueTypePairList list)
        {
            if (list.Capacity != 0)
                UnsafeUtility.Free(list.Values, Allocator.Temp);
            list = default;
        }
    }
}
