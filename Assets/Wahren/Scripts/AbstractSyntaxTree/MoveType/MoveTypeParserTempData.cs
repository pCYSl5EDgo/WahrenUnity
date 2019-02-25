using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct MovetypeParserTempData : IParserTempData
    {
        public ListLinkedList Values;
        public ListLinkedList Names;
        public ListLinkedList Helps;
        public ListLinkedList Constis;
        public IdentifierNumberPairListLinkedList IdentifierNumberPairs2;

        public MovetypeParserTempData(int capacity, Allocator allocator)
        {
            IdentifierNumberPairs2 = new IdentifierNumberPairListLinkedList(capacity, allocator);
            if (capacity != 0)
            {
                Names = new ListLinkedList(capacity, sizeof(MovetypeTree.NameAssignExpression), allocator);
                Helps = new ListLinkedList(capacity, sizeof(MovetypeTree.HelpAssignExpression), allocator);
                Constis = new ListLinkedList(capacity, sizeof(MovetypeTree.ConstiAssignExpression), allocator);
                Values = new ListLinkedList(capacity, sizeof(MovetypeTree), allocator);
            }
            else
            {
                Values = Names = Helps = Constis = default;
            }
        }

        public void Dispose(Allocator allocator)
        {
            IdentifierNumberPairs2.Dispose(allocator);
            Values.Dispose(allocator);
            Names.Dispose(allocator);
            Helps.Dispose(allocator);
            Constis.Dispose(allocator);
            this = default;
        }

        public void Lengthen(ref ASTTypePageIndexPairList astValueTypePairList, in TryInterpretReturnValue result, Allocator allocator
#if UNITY_EDITOR
        , bool ShowLog
#endif
        )
        {
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
            }
        }
    }
}