using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct MovetypeParserTempData : IParserTempData
    {
        public int Length;

        public int Capacity;

        public MovetypeTree* Values;
        public ListLinkedList Values2;
        public ListLinkedList Names2;
        public ListLinkedList Helps2;
        public ListLinkedList Constis2;
        public IdentifierNumberPairList IdentifierNumberPairs;
        public IdentifierNumberPairListLinkedList IdentifierNumberPairs2;

        public MovetypeParserTempData(int capacity, Allocator allocator)
        {
            Length = 0;
            Capacity = capacity;
            IdentifierNumberPairs = new IdentifierNumberPairList(capacity, allocator);
            IdentifierNumberPairs2 = new IdentifierNumberPairListLinkedList(capacity, allocator);
            if (capacity != 0)
            {
                Values = (MovetypeTree*)UnsafeUtility.Malloc(sizeof(MovetypeTree) * capacity, 4, allocator);
                Names2 = new ListLinkedList(capacity, sizeof(MovetypeTree.NameAssignExpression), allocator);
                Helps2 = new ListLinkedList(capacity, sizeof(MovetypeTree.HelpAssignExpression), allocator);
                Constis2 = new ListLinkedList(capacity, sizeof(MovetypeTree.ConstiAssignExpression), allocator);
                Values2 = new ListLinkedList(capacity, sizeof(MovetypeTree), allocator);
            }
            else
            {
                Values = null;
                Values2 = Names2 = Helps2 = Constis2 = default;
            }
        }

        public void Dispose(Allocator allocator)
        {
            if (Capacity != 0)
                UnsafeUtility.Free(Values, allocator);
            IdentifierNumberPairs.Dispose(allocator);
            IdentifierNumberPairs2.Dispose(allocator);
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
                    throw new System.Exception();
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