namespace pcysl5edgo.Wahren
{
    public enum InterpreterStatus
    {
        None, Success, Error, Pending,
    }
    public struct TryInterpretReturnValue
    {
        public Span Span;
        public int DataIndex;
        public int SubDataIndex;
        public InterpreterStatus Status;
        public long SubdData0;
        public long SubdData1;

        public bool IsSuccess => Status == InterpreterStatus.Success;
        public bool IsError => Status == InterpreterStatus.Error;
        public bool IsPending => Status == InterpreterStatus.Pending;

        public TryInterpretReturnValue(Caret start, int dataIndex, InterpreterStatus status)
        {
            this.Span = new Span { Start = start, Length = 0 };
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = 0;
            SubdData0 = SubdData1 = 0;
        }
        public TryInterpretReturnValue(Caret start, int dataIndex, int subDataIndex, InterpreterStatus status)
        {
            this.Span = new Span { Start = start, Length = 0 };
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = subDataIndex;
            SubdData0 = SubdData1 = 0;
        }
        public TryInterpretReturnValue(Span span, int dataIndex, InterpreterStatus status)
        {
            this.Span = span;
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = 0;
            SubdData0 = SubdData1 = 0;
        }
        public TryInterpretReturnValue(Span span, int dataIndex, int subDataIndex, InterpreterStatus status)
        {
            this.Span = span;
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = subDataIndex;
            SubdData0 = SubdData1 = 0;
        }

        public static TryInterpretReturnValue CreateSuccessDetectStructType(in Caret caret, int length, AST.Location location) => new TryInterpretReturnValue
        {
            Span = new Span(caret, length),
            DataIndex = SuccessSentence.StructKindInterpretSuccess,
            SubDataIndex = (int)location,
            SubdData0 = 0,
            SubdData1 = 0,
        };

        public static TryInterpretReturnValue CreatePending(Span span, AST.Location location, AST.PendingReason reason, long subData0 = 0, long subData1 = 0) => new TryInterpretReturnValue
        {
            Span = span,
            DataIndex = ((byte)location << 24) | (byte)reason,
            SubdData0 = subData0,
            SubdData1 = subData1,
            Status = InterpreterStatus.Pending,
        };

        public static implicit operator bool(in TryInterpretReturnValue value) => value.Status == InterpreterStatus.Success;
    }
}