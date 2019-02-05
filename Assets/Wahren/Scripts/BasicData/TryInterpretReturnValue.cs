﻿namespace pcysl5edgo.Wahren
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
        public long SubdData0;
        public long SubdData1;

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

        public static TryInterpretReturnValue CreatePending(Span span, int dataIndex, int subDataIndex, long subData0, long subData1)
        => new TryInterpretReturnValue
        {
            Span = span,
            DataIndex = dataIndex,
            SubDataIndex = subDataIndex,
            SubdData0 = subData0,
            SubdData1 = subData1,
        };
        public static TryInterpretReturnValue CreatePending(Caret caret, int dataIndex, int subDataIndex, long subData0, long subData1)
        => new TryInterpretReturnValue
        {
            Span = new Span(caret, 0),
            DataIndex = dataIndex,
            SubDataIndex = subDataIndex,
            SubdData0 = subData0,
            SubdData1 = subData1,
        };
    }
}