using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct NativeString
    {
        public long Start;
        public long Length;
        public int File;
        public int LineStartInclusive;
        public int LineEndExclusive;
    }
    public unsafe struct NativeStringList
    {
        public int Capacity;
        public int Length;
        public ushort* Values;

        public NativeStringList(int capacity)
        {
            Length = 0;
            Capacity = capacity;
            if (Capacity == 0)
                Values = null;
            else
                Values = (ushort*)UnsafeUtility.Malloc(sizeof(ushort) * capacity, 2, Allocator.Persistent);
        }

        public void Dispose()
        {
            if (Capacity != 0)
            {
                UnsafeUtility.Free(Values, Allocator.Persistent);
                this = default;
            }
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
            int size = sizeof(ushort) * Capacity;
            var _ = UnsafeUtility.Malloc(size * 2, 4, allocator);
            UnsafeUtility.MemCpy(_, Values, size);
            UnsafeUtility.Free(Values, allocator);
            Values = (ushort*)_;
            Capacity *= 2;
        }
    }
}