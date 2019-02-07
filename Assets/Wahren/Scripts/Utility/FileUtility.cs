namespace pcysl5edgo.Wahren
{
    public static unsafe class TextFileUtility
    {
        public static ushort CurrentChar(this in TextFile file, Caret caret) => file.Lines[caret.Line][caret.Column];
        public static ushort* CurrentLine(this in TextFile file, Caret caret) => file.Lines[caret.Line];
        public static ushort* CurrentCharPointer(this in TextFile file, Caret caret) => file.Lines[caret.Line] + caret.Column;
        public static int CurrentLineLength(this in TextFile file, Caret caret) => file.LineLengths[caret.Line];
    }
}