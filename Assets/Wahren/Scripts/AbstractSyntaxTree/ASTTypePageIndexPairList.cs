using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct ASTTypePageIndexPairList
    {
        public ASTTypePageIndexPair* Values;
        public int Capacity;
        public int Length;

        public ASTTypePageIndexPairList(int capacity, Allocator allocator)
        {
            if (capacity == 0)
            {
                this = default;
                return;
            }
            Capacity = capacity;
            Length = 0;
            Values = (ASTTypePageIndexPair*)UnsafeUtility.Malloc(sizeof(ASTTypePageIndexPair) * capacity, 4, allocator);
        }

        public ref ASTTypePageIndexPair this[int index]
        {
            get
            {
                if (index >= Length || index < 0)
                {
                    throw new System.ArgumentOutOfRangeException();
                }
                return ref Values[index];
            }
        }

        public int TryAddBulkMultiThread(in ASTTypePageIndexPairList list, out int length)
        {
            if (list.Length == 0)
            {
                length = 0;
                return 0;
            }
            int start = -1;
            length = list.Length;
            do
            {
                start = Length;
                if (start + length > Capacity)
                {
                    length = 0;
                    return -1;
                }
            } while (start != Interlocked.CompareExchange(ref Length, start + length, start));
            UnsafeUtility.MemCpy(Values + start, list.Values, sizeof(ASTTypePageIndexPair) * length);
            return start;
        }

        public void Dispose(Allocator allocator)
        {
            if (Capacity != 0)
                UnsafeUtility.Free(Values, allocator);
            this = default;
        }

        public static ASTTypePageIndexPairList MallocTemp(int capacity)
        => new ASTTypePageIndexPairList
        {
            Capacity = capacity,
            Length = 0,
            Values = (ASTTypePageIndexPair*)UnsafeUtility.Malloc(sizeof(ASTTypePageIndexPair) * capacity, 4, Allocator.Temp),
        };
        public static void FreeTemp(ref ASTTypePageIndexPairList list)
        {
            if (list.Capacity != 0)
                UnsafeUtility.Free(list.Values, Allocator.Temp);
            list = default;
        }
    }
}
