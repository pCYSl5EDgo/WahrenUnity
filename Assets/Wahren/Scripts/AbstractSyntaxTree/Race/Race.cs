namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct RaceTree : INameStruct
    {
        public Span Name;
        public Span ParentName;

        public ASTTypePageIndexPairList* Page;
        public int Start;
        public int Length;

        internal enum Kind
        {
            name,
            align,
            brave,
            consti,
            movetype,
        }

        public struct NameAssignExpression // 0
        {
            public Span ScenarioVariant;
            public Span Value;
        }
        public struct AlignAssignExpression // 1
        {
            public Span ScenarioVariant;
            public sbyte Value;
        }
        public struct BraveAssignExpression // 2
        {
            public Span ScenarioVariant;
            public sbyte Value;
        }
        public unsafe struct ConstiAssignExpression // 3
        {
            public Span ScenarioVariant;
            public IdentifierNumberPairList* Page;
            public int Start;
            public int Length;
        }
        public struct MovetypeAssignExpression // 4
        {
            public Span ScenarioVariant;
            public Span Value;
        }

        public void SetNameAndParentName(Span name, Span parentName)
        {
            Name = name;
            ParentName = parentName;
        }
    }
}