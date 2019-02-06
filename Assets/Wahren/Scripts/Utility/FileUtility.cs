namespace pcysl5edgo.Wahren
{
    public static unsafe class TextFileUtility
    {
        public static char CurrentChar(this in TextFile file, Caret caret) => (char)file.Lines[caret.Line][caret.Column];
        public static char* CurrentLine(this in TextFile file, Caret caret) => (char*)file.Lines[caret.Line];
        public static char* CurrentCharPointer(this in TextFile file, Caret caret) => (char*)file.Lines[caret.Line] + caret.Column;
        public static int CurrentLineLength(this in TextFile file, Caret caret) => file.LineLengths[caret.Line];
    }
}