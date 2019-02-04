namespace pcysl5edgo.Wahren
{
    public enum InterpreterStatus
    {
        None, Success, Error, Pending,
    }
    public struct TryInterpretReturnValue
    {
        private static readonly System.Text.StringBuilder buffer = new System.Text.StringBuilder(4096);
        public Span Span;
        public int DataIndex;
        public int SubDataIndex;
        public InterpreterStatus Status;
        public bool IsSuccess => Status == InterpreterStatus.Success;
        public bool IsError => Status == InterpreterStatus.Error;
        public bool IsPending => Status == InterpreterStatus.Pending;

        public string ToString(in ScriptLoadReturnValue script) => buffer.Clear().Append(this, script).ToString();
        public TryInterpretReturnValue(Caret start, int dataIndex, InterpreterStatus status)
        {
            this.Span = new Span { Start = start, Length = 0 };
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = 0;
        }
        public TryInterpretReturnValue(Caret start, int dataIndex, int subDataIndex, InterpreterStatus status)
        {
            this.Span = new Span { Start = start, Length = 0 };
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = subDataIndex;
        }
        public TryInterpretReturnValue(Span span, int dataIndex, InterpreterStatus status)
        {
            this.Span = span;
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = 0;
        }
        public TryInterpretReturnValue(Span span, int dataIndex, int subDataIndex, InterpreterStatus status)
        {
            this.Span = span;
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = subDataIndex;
        }
    }
}