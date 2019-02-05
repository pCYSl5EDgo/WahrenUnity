namespace pcysl5edgo.Wahren
{
    public unsafe struct IdentifierNumberPair
    {
        public Span Span;
        public long Number;
        public Span NumberSpan;

        public IdentifierNumberPair(Span span, long number, Span numberSpan
        )
        {
            Span = span;
            Number = number;
            NumberSpan = numberSpan;
        }
    }
}