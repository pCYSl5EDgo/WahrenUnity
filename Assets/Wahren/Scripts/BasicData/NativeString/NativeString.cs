using System;
using System.Threading;
using Unity.Collections;

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

        public bool IsEmpty => Values == null && ColumnEndExclusive == 0;
        public bool IsAtmark => Values == null && ColumnEndExclusive != 0;
    }
}