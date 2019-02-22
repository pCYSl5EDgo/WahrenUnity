using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct MovetypeParserTempData : IParserTempData
    {
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

        public MovetypeParserTempData(int capacity, Allocator allocator)
        {
            Length = NameLength = HelpLength = ConstiLength = 0;
            Capacity = NameCapacity = HelpCapacity = ConstiCapacity = capacity;
            IdentifierNumberPairs = new IdentifierNumberPairList(capacity, allocator);
            if (capacity != 0)
            {
                Values = (MovetypeTree*)UnsafeUtility.Malloc(sizeof(MovetypeTree) * capacity, 4, allocator);
                Names = (MovetypeTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(MovetypeTree.NameAssignExpression) * capacity, 4, allocator);
                Helps = (MovetypeTree.HelpAssignExpression*)UnsafeUtility.Malloc(sizeof(MovetypeTree.HelpAssignExpression) * capacity, 4, allocator);
                Constis = (MovetypeTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(MovetypeTree.ConstiAssignExpression) * capacity, 4, allocator);
            }
            else
            {
                Values = null;
                Names = null;
                Helps = null;
                Constis = null;
            }
        }

        public void Dispose(Allocator allocator)
        {
            if (NameCapacity != 0)
                UnsafeUtility.Free(Names, allocator);
            if (HelpCapacity != 0)
                UnsafeUtility.Free(Helps, allocator);
            if (ConstiCapacity != 0)
                UnsafeUtility.Free(Constis, allocator);
            if (Capacity != 0)
                UnsafeUtility.Free(Values, allocator);
            IdentifierNumberPairs.Dispose(allocator);
            this = default;
        }

        public void Lengthen(ref ASTTypePageIndexPairList astValueTypePairList, in TryInterpretReturnValue result, Allocator allocator
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
                    ListUtility.Lengthen(ref astValueTypePairList.Values, ref astValueTypePairList.Capacity, allocator);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
#if UNITY_EDITOR
                    if (ShowLog)
                    {
                        UnityEngine.Debug.Log(prefix + " identifier number pair lengthen\n" + result.ToString() + "\n" + identifierNumberPairs.Capacity);
                    }
#endif
                    identifierNumberPairs.Lengthen(allocator);
                    break;
                case PendingReason.SectionListCapacityShortage:
                    switch ((MovetypeTree.Kind)result.SubDataIndex)
                    {
                        case MovetypeTree.Kind.name: // name
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " name lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Names, ref NameCapacity, allocator);
                            break;
                        case MovetypeTree.Kind.help: // help
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " help lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Helps, ref HelpCapacity, allocator);
                            break;
                        case MovetypeTree.Kind.consti: // consti
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " consti lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Constis, ref ConstiCapacity, allocator);
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
                    ListUtility.Lengthen(ref Values, ref Capacity, allocator);
                    break;
            }
        }
    }
}