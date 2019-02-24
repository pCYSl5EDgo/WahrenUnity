using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct MovetypeParserTempData : IParserTempData
    {
        public int Length;
        public int HelpLength;
        public int ConstiLength;

        public int Capacity;
        public int HelpCapacity;
        public int ConstiCapacity;

        public MovetypeTree* Values;
        public MovetypeTree.HelpAssignExpression* Helps;
        public MovetypeTree.ConstiAssignExpression* Constis;
        public ListLinkedList Values2;
        public ListLinkedList Names2;
        public ListLinkedList Helps2;
        public ListLinkedList Constis2;
        public IdentifierNumberPairList IdentifierNumberPairs;

        public MovetypeParserTempData(int capacity, Allocator allocator)
        {
            Length  = HelpLength = ConstiLength = 0;
            Capacity = HelpCapacity = ConstiCapacity = capacity;
            IdentifierNumberPairs = new IdentifierNumberPairList(capacity, allocator);
            if (capacity != 0)
            {
                Values = (MovetypeTree*)UnsafeUtility.Malloc(sizeof(MovetypeTree) * capacity, 4, allocator);
                Helps = (MovetypeTree.HelpAssignExpression*)UnsafeUtility.Malloc(sizeof(MovetypeTree.HelpAssignExpression) * capacity, 4, allocator);
                Constis = (MovetypeTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(MovetypeTree.ConstiAssignExpression) * capacity, 4, allocator);
                Names2 = new ListLinkedList(capacity, sizeof(MovetypeTree.NameAssignExpression), allocator);
                Helps2 = new ListLinkedList(capacity, sizeof(MovetypeTree.HelpAssignExpression),allocator);
                Constis2 = new ListLinkedList(capacity, sizeof(MovetypeTree.ConstiAssignExpression), allocator);
                Values2 = new ListLinkedList(capacity, sizeof(MovetypeTree),allocator);
            }
            else
            {
                Values = null;
                Helps = null;
                Constis = null;
                Values2 = Names2 = Helps2 = Constis2 = default;
            }
        }

        public void Dispose(Allocator allocator)
        {
            if (HelpCapacity != 0)
                UnsafeUtility.Free(Helps, allocator);
            if (ConstiCapacity != 0)
                UnsafeUtility.Free(Constis, allocator);
            if (Capacity != 0)
                UnsafeUtility.Free(Values, allocator);
            IdentifierNumberPairs.Dispose(allocator);
            Values2.Dispose(allocator);
            Names2.Dispose(allocator);
            Helps2.Dispose(allocator);
            Constis2.Dispose(allocator);
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
                    ListUtility.Lengthen(ref astValueTypePairList.This.Values, ref astValueTypePairList.This.Capacity, allocator);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
#if UNITY_EDITOR
                    if (ShowLog)
                    {
                        UnityEngine.Debug.Log(prefix + " identifier number pair lengthen\n" + result.ToString() + "\n" + identifierNumberPairs.This.Capacity);
                    }
#endif
                    identifierNumberPairs.Lengthen(allocator);
                    break;
                case PendingReason.SectionListCapacityShortage:
                    switch ((MovetypeTree.Kind)result.SubDataIndex)
                    {
                        case MovetypeTree.Kind.name: // name
                            throw new System.Exception();
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