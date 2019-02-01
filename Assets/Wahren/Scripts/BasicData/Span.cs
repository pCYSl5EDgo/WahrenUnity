using System;
using System.Runtime.InteropServices;

namespace pcysl5edgo.Wahren
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Span : IEquatable<Span>
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
        public override bool Equals(object obj) => !(obj is null) && Equals((Span)obj);
        public bool Equals(Span span) => File == span.File && Line == span.Line && Column == span.Column && Length == span.Length;
        public static bool operator ==(in Span span0, in Span span1) => span0.File == span1.File && span0.Line == span1.Line && span0.Column == span1.Column && span0.Length == span1.Length;
        public static bool operator !=(in Span span0, in Span span1) => span0.File != span1.File || span0.Line != span1.Line || span0.Column != span1.Column || span0.Length != span1.Length;
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