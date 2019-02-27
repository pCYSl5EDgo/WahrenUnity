using System;
using System.Threading;
using Unity.Collections;

using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct NativeString
    {
        public ushort* Values;
        public int Length;
        public int File;
        public int LineStartInclusive;
        public int ColumnStartInclusive;
        public int LineEndInclusive;
        public int ColumnEndExclusive;
    }
}