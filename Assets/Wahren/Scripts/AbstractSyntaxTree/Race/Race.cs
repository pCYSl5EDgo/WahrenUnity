﻿namespace pcysl5edgo.Wahren.AST
{
    public struct RaceTree
    {
        public Span Name;
        public Span ParentName;

        public int Start;
        public int Length;

        internal const int name = 0, align = 1, brave = 2, consti = 3, movetype = 4;

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
            public int Start;
            public int Length;
        }
        public struct MoveTypeAssignExpression // 4
        {
            public Span ScenarioVariant;
            public Span Value;
        }
    }
}