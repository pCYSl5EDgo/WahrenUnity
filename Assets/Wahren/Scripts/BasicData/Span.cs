﻿using System;
using System.Runtime.InteropServices;

namespace pcysl5edgo.Wahren
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Span
    {
        [FieldOffset(0)]
        public int File;
        [FieldOffset(4)]
        public int Line;
        [FieldOffset(8)]
        public int Column;
        [FieldOffset(12)]
        public int Length;
        [FieldOffset(0)]
        public Caret Start;

        public Caret CaretNextToEndOfThisSpan => new Caret { File = this.File, Line = this.Line, Column = this.Column + this.Length };

        public override string ToString() => $"({File}, {Line}, {Column}, {Length})";
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Caret
    {
        [FieldOffset(0)]
        public int File;
        [FieldOffset(4)]
        public int Line;
        [FieldOffset(8)]
        public int Column;

        public override string ToString() => $"({File}, {Line}, {Column})";
    }
}