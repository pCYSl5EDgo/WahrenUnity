using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct MovetypeParserTempData : IParserTempData
    {
        internal struct OldLengths
        {
#pragma warning disable CS0649
            public int Length;
            public int NameLength;
            public int HelpLength;
            public int ConstiLength;
#pragma warning restore

            public static bool IsChanged(OldLengths* left, MovetypeParserTempData* right)
            => UnsafeUtility.MemCmp(left, right, sizeof(OldLengths)) == 0;
        }
        public int Length;
        public int NameLength;
        public int HelpLength;
        public int ConstiLength;

        public int Capacity;
        public int NameCapacity;
        public int HelpCapacity;
        public int ConstiCapacity;

        public MovetypeTree* Values;
        public MovetypeTree.NameAssignExpression* Names;
        public MovetypeTree.HelpAssignExpression* Helps;
        public MovetypeTree.ConstiAssignExpression* Constis;
        public IdentifierNumberPairList IdentifierNumberPairs;

        public MovetypeParserTempData(int capacity)
        {
            Length = NameLength = HelpLength = ConstiLength = 0;
            Capacity = NameCapacity = HelpCapacity = ConstiCapacity = capacity;
            IdentifierNumberPairs = new IdentifierNumberPairList(capacity);
            if (capacity != 0)
            {
                Values = (MovetypeTree*)UnsafeUtility.Malloc(sizeof(MovetypeTree) * capacity, 4, Allocator.Persistent);
                Names = (MovetypeTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(MovetypeTree.NameAssignExpression) * capacity, 4, Allocator.Persistent);
                Helps = (MovetypeTree.HelpAssignExpression*)UnsafeUtility.Malloc(sizeof(MovetypeTree.HelpAssignExpression) * capacity, 4, Allocator.Persistent);
                Constis = (MovetypeTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(MovetypeTree.ConstiAssignExpression) * capacity, 4, Allocator.Persistent);
            }
            else
            {
                Values = null;
                Names = null;
                Helps = null;
                Constis = null;
            }
        }

        public void Dispose()
        {
            if (NameCapacity != 0)
                UnsafeUtility.Free(Names, Allocator.Persistent);
            if (HelpCapacity != 0)
                UnsafeUtility.Free(Helps, Allocator.Persistent);
            if (ConstiCapacity != 0)
                UnsafeUtility.Free(Constis, Allocator.Persistent);
            if (Capacity != 0)
                UnsafeUtility.Free(Values, Allocator.Persistent);
            IdentifierNumberPairs.Dispose();
            this = default;
        }

        public void Lengthen(ref ASTValueTypePairList astValueTypePairList, in TryInterpretReturnValue result
#if UNITY_EDITOR
        , bool ShowLog
#endif
        )
        {
            ref var identifierNumberPairs = ref IdentifierNumberPairs;
            (_, var reason) = result;
            const string prefix = "movetype";
            switch (reason)
            {
                case PendingReason.ASTValueTypePairListCapacityShortage:
#if UNITY_EDITOR
                    if (ShowLog)
                    {
                        UnityEngine.Debug.Log(prefix + " ast value type pair lengthen\n" + result.ToString());
                    }
#endif
                    ListUtility.Lengthen(ref astValueTypePairList.Values, ref astValueTypePairList.Capacity);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
#if UNITY_EDITOR
                    if (ShowLog)
                    {
                        UnityEngine.Debug.Log(prefix + " identifier number pair lengthen\n" + result.ToString() + "\n" + identifierNumberPairs.Capacity);
                    }
#endif
                    identifierNumberPairs.Lengthen();
                    break;
                case PendingReason.SectionListCapacityShortage:
                    switch (result.SubDataIndex)
                    {
                        case MovetypeTree.name + 1: // name
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " name lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Names, ref NameCapacity);
                            break;
                        case MovetypeTree.help + 1: // help
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " help lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Helps, ref HelpCapacity);
                            break;
                        case MovetypeTree.consti + 1: // consti
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " consti lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Constis, ref ConstiCapacity);
                            break;
                    }
                    break;
                case PendingReason.TreeListCapacityShortage:
#if UNITY_EDITOR
                    if (ShowLog)
                    {
                        UnityEngine.Debug.Log(prefix + " lengthen\n" + result.ToString());
                    }
#endif
                    ListUtility.Lengthen(ref Values, ref Capacity);
                    break;
            }
        }
    }
}