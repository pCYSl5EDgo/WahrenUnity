namespace pcysl5edgo.Wahren
{
    public static unsafe class TextFileUtility
    {
        public static ushort CurrentChar(this TextFile file, Caret caret) => (file.Contents + file.LineStarts[caret.Line])[caret.Column];
        public static ushort* CurrentLine(this TextFile file, Caret caret) => file.Contents + file.LineStarts[caret.Line];
        public static ushort* CurrentCharPointer(this TextFile file, Caret caret) => file.Contents + file.LineStarts[caret.Line] + caret.Column;
        public static int CurrentLineLength(this TextFile file, Caret caret) => file.LineLengths[caret.Line];
    }
}