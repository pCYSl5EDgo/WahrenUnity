using System;
using System.Runtime.InteropServices;

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
    public struct Token
    {
        public TokenKind Kind;
        public Span Span;
    }

    public enum TokenKind
    {
        None, Number, Alphabet, Symbol, Space, CRLF,
    }
}