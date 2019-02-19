namespace pcysl5edgo.Wahren.AST
{
    public struct MovetypeTree : INameStruct
    {
        public Span Name;
        public Span ParentName;

        public int Start;
        public int Length;

        internal const int name = 0, help = 1, consti = 2;

        public struct NameAssignExpression // 0
        {
            public Span ScenarioVariant;
            public Span Value;
        }

        public struct HelpAssignExpression // 1
        {
            public Span ScenarioVariant;
            public Span Value;
        }

        public unsafe struct ConstiAssignExpression // 2
        {
            public Span ScenarioVariant;
            public int Start;
            public int Length;
        }

        public void SetNameAndParentName(Span name, Span parentName)
        {
            Name = name;
            ParentName = parentName;
        }
    }
}