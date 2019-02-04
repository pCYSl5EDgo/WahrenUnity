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
        public bool TryAddMultiThread(in ASTValueTypePair pair)
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
            if (Capacity == 0)
            {
                this = new ASTValueTypePairList(1);
                return;
            }
            var _ = new ASTValueTypePairList(Capacity * 2);
            UnsafeUtility.MemCpy(_.Values, Values, sizeof(ASTValueTypePair) * Capacity);
            UnsafeUtility.Free(Values, Allocator.Persistent);
            Values = _.Values;
            Capacity = _.Capacity;
        }

        public void Dispose()
        {
            if (Capacity != 0)
                UnsafeUtility.Free(Values, Allocator.Persistent);
            this = default;
        }
    }
}
