using System;

namespace pcysl5edgo.Wahren
{
    public struct Span
    {
        public int File;
        public int Line;
        public int Column;
        public int Length;

        public override string ToString() => $"({File}, {Line}, {Column}, {Length})";
    }
}