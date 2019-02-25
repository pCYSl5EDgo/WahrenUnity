using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct RaceParserTempData : IParserTempData
    {
        public int Length;
        public int Capacity;
        public RaceTree* Values;
        public ListLinkedList Names;
        public ListLinkedList Aligns;
        public ListLinkedList Braves;
        public ListLinkedList Movetypes;
        public ListLinkedList Constis;
        public IdentifierNumberPairListLinkedList IdentifierNumberPairs2;

        public RaceParserTempData(int capacity, Allocator allocator)
        {
            this = default;
            Length = 0;
            Capacity = capacity;
            IdentifierNumberPairs2 = new IdentifierNumberPairListLinkedList(capacity, allocator);
            if (capacity != 0)
            {
                Values = (RaceTree*)UnsafeUtility.Malloc(sizeof(RaceTree) * capacity, 4, allocator);
                Names = new ListLinkedList(capacity, sizeof(RaceTree.NameAssignExpression), allocator);
                Aligns = new ListLinkedList(capacity, sizeof(RaceTree.AlignAssignExpression), allocator);
                Braves = new ListLinkedList(capacity, sizeof(RaceTree.BraveAssignExpression), allocator);
                Movetypes = new ListLinkedList(capacity, sizeof(RaceTree.MovetypeAssignExpression), allocator);
                Constis = new ListLinkedList(capacity, sizeof(RaceTree.ConstiAssignExpression), allocator);
            }
        }

        public void Dispose(Allocator allocator)
        {
            if (Capacity != 0)
                UnsafeUtility.Free(Values, allocator);
            IdentifierNumberPairs2.Dispose(allocator);
            Names.Dispose(allocator);
            Aligns.Dispose(allocator);
            Braves.Dispose(allocator);
            Movetypes.Dispose(allocator);
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
            const string prefix = "race";
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