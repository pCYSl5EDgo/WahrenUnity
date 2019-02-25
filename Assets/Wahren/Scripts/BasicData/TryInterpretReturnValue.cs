namespace pcysl5edgo.Wahren.AST
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
        public long SubData0;
        public long SubData1;

        public bool IsSuccess => Status == InterpreterStatus.Success;
        public bool IsError => Status == InterpreterStatus.Error;
        public bool IsPending => Status == InterpreterStatus.Pending;

        public TryInterpretReturnValue(Caret start, int dataIndex, InterpreterStatus status)
        {
            this.Span = new Span { Start = start, Length = 0 };
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = 0;
            SubData0 = SubData1 = 0;
        }
        public TryInterpretReturnValue(Caret start, int dataIndex, int subDataIndex, InterpreterStatus status)
        {
            this.Span = new Span { Start = start, Length = 0 };
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = subDataIndex;
            SubData0 = SubData1 = 0;
        }
        public TryInterpretReturnValue(Span span, int dataIndex, InterpreterStatus status)
        {
            this.Span = span;
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = 0;
            SubData0 = SubData1 = 0;
        }
        public TryInterpretReturnValue(Span span, int dataIndex, int subDataIndex, InterpreterStatus status)
        {
            this.Span = span;
            this.Status = status;
            this.DataIndex = dataIndex;
            this.SubDataIndex = subDataIndex;
            this.SubData0 = this.SubData1 = 0;
        }
        public TryInterpretReturnValue(Span span, SuccessSentence.Kind kind, int subDataIndex = 0) : this(span, (int)kind, subDataIndex, InterpreterStatus.Success) { }
        public TryInterpretReturnValue(Caret start, SuccessSentence.Kind kind, int subDataIndex = 0) : this(start, (int)kind, subDataIndex, InterpreterStatus.Success) { }
        public TryInterpretReturnValue(Span span, ErrorSentence.Kind kind, int subDataIndex = 0) : this(span, (int)kind, subDataIndex, InterpreterStatus.Error) { }
        public TryInterpretReturnValue(Caret start, ErrorSentence.Kind kind, int subDataIndex = 0) : this(start, (int)kind, subDataIndex, InterpreterStatus.Error) { }

        public static TryInterpretReturnValue CreateSuccessDetectStructType(in Caret caret, int length, Location location) => new TryInterpretReturnValue(new Span(caret, length), SuccessSentence.Kind.StructKindInterpretSuccess, (int)location);

        public static TryInterpretReturnValue CreateRightBraceNotFound(Caret current) => new TryInterpretReturnValue(current, ErrorSentence.Kind.ExpectedCharNotFoundError, 2);
        public static TryInterpretReturnValue CreateNotExpectedCharacter(Caret current) => new TryInterpretReturnValue
        {
            Span = new Span
            {
                Start = current,
                Length = 1,
            },
            DataIndex = (int)ErrorSentence.Kind.NotExpectedCharacterError,
            Status = InterpreterStatus.Error,
        };

        public static implicit operator bool(in TryInterpretReturnValue value) => value.Status == InterpreterStatus.Success;

        public override string ToString()
        {
            var buffer = new System.Text.StringBuilder(128);
            buffer.Append(Span.ToString()).Append(", ").AppendLine(Status.ToString()).Append(DataIndex).Append(", ").Append(SubDataIndex).AppendLine().Append(SubData0).AppendLine().Append(SubData1);
            return buffer.ToString();
        }
    }
}