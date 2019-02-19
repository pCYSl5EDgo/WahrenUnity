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

        public static TryInterpretReturnValue CreateSuccessDetectStructType(in Caret caret, int length, Location location) => new TryInterpretReturnValue(new Span(caret, length), SuccessSentence.StructKindInterpretSuccess, (int)location, InterpreterStatus.Success);

        public static TryInterpretReturnValue CreatePending(Span span, Location location, PendingReason reason, int subDataIndex = 0) => new TryInterpretReturnValue
        {
            Span = span,
            DataIndex = 0,
            SubDataIndex = subDataIndex,
            SubData0 = (long)location,
            SubData1 = (long)reason,
            Status = InterpreterStatus.Pending,
        };

        public static TryInterpretReturnValue CreateRightBraceNotFound(Caret current) => new TryInterpretReturnValue(current, ErrorSentence.ExpectedCharNotFoundError, 2, InterpreterStatus.Error);
        public static TryInterpretReturnValue CreateNotExpectedCharacter(Caret current) => new TryInterpretReturnValue
        {
            Span = new Span
            {
                Start = current,
                Length = 1,
            },
            DataIndex = ErrorSentence.NotExpectedCharacterError,
            Status = InterpreterStatus.Error,
        };

        public void Deconstruct(out Location location, out PendingReason reason)
        {
            location = (Location)SubData0;
            reason = (PendingReason)SubData1;
        }

        public static implicit operator bool(in TryInterpretReturnValue value) => value.Status == InterpreterStatus.Success;

        public override string ToString()
        {
            var buffer = new System.Text.StringBuilder(128);
            buffer.Append(Span.ToString()).Append(", ").AppendLine(Status.ToString());
            if (Status == InterpreterStatus.Pending)
            {
                var (location, reason) = this;
                buffer.Append(location).Append(", ").Append(reason).AppendLine().Append(SubDataIndex);
            }
            else
            {
                buffer.Append(DataIndex).Append(", ").Append(SubDataIndex).AppendLine().Append(SubData0).AppendLine().Append(SubData1);
            }
            return buffer.ToString();
        }
    }
}