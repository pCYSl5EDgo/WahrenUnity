namespace pcysl5edgo.Wahren
{
    public unsafe struct IdentifierNumberPair
    {
        public Span Span;
        public long Number;
#if UNITY_EDITOR
        public Span NumberSpan;
#endif

        public IdentifierNumberPair(Span span, long number
#if UNITY_EDITOR
        , Span numberSpan
#endif        
        )
        {
            Span = span;
            Number = number;
#if UNITY_EDITOR
            NumberSpan = numberSpan;
#endif
        }
    }
}