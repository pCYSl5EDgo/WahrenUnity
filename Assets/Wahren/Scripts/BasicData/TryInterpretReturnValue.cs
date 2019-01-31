namespace pcysl5edgo.Wahren
{
    public struct TryInterpretReturnValue
    {
        private static readonly System.Text.StringBuilder buffer = new System.Text.StringBuilder(4096);
        public Span Span;
        public int DataIndex;
        public int SubDataIndex;
        public byte isSuccess;
        public bool IsSuccess { get => isSuccess != 0; set => isSuccess = (byte)(value ? 1 : 0); }

        public string ToString(ref ScriptLoadReturnValue script)
        {
            buffer.Clear();
            if (!IsSuccess)
            {
                buffer.AppendLine(ErrorSentence.Contents[DataIndex]);
                if (ErrorSentence.SubContents[DataIndex] != null && ErrorSentence.SubContents[DataIndex].Length != 0)
                    buffer.AppendLine(ErrorSentence.SubContents[DataIndex][SubDataIndex]);
            }
            buffer.Append("@File: ").Append(script.FullPaths[Span.File]).Append(" in line ").Append(Span.Line + 1).Append('(').Append(Span.Column + 1).Append('-').Append(Span.Column + 1 + Span.Length).Append(")\n").Append(script.ToString(ref Span));
            return buffer.ToString();
        }

        public TryInterpretReturnValue(ref Span errorLocation, int dataIndex, int subDataIndex, bool isSuccess)
        {
            this.Span = errorLocation;
            this.isSuccess = (byte)(isSuccess ? 1 : 0);
            this.DataIndex = dataIndex;
            this.SubDataIndex = subDataIndex;
        }
    }
}