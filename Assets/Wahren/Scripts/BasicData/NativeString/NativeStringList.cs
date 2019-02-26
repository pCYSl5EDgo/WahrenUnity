using System;
using System.Threading;
using Unity.Collections;

using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct NativeString
    {
        public long Start;
        public long Length;
        public int File;
        public int LineStartInclusive;
        public int ColumnStartInclusive;
        public int LineEndInclusive;
        public int ColumnEndExclusive;
    }
    public unsafe struct NativeStringList
    {
        public long Capacity;
        public long Length;
        public ushort* Values;

        public NativeStringList(long capacity)
        {
            Length = 0;
            Capacity = capacity;
            if (Capacity == 0)
                Values = null;
            else
                Values = (ushort*)Malloc(sizeof(ushort) * capacity, 2, Allocator.Persistent);
        }

        public void Dispose()
        {
            if (Capacity != 0)
            {
                Free(Values, Allocator.Persistent);
            }
            this = default;
        }
    }
}