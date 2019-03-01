namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct VoiceTree : INameStruct
    {
        public Span Name;
        public Span ParentName;

        public ASTTypePageIndexPairList* Page;
        public int Start;
        public int Length;

        internal enum Kind
        {
            voice_type,
            delskill,
            power,
            spot,
        }

        public struct VoiceTypeAssignExpression
        {
            public Span ScenarioVariant;
            public IdentifierList* Page;
            public int Start;
            public int Length;
        }

        public struct DelskillAssignExpression
        {
            public Span ScenarioVariant;
            public IdentifierList* Page;
            public int Start;
            public int Length;
        }

        public struct PowerAssignExpression
        {
            public Span ScenarioVariant;
            public NativeString Value;
        }

        public struct SpotAssignExpression
        {
            public Span ScenarioVariant;
            public NativeString Value;
        }

        public void SetNameAndParentName(Span name, Span parentName)
        {
            Name = name;
            ParentName = parentName;
        }
    }
}