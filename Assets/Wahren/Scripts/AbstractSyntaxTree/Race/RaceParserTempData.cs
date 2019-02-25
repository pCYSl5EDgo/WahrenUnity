using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct RaceParserTempData : IParserTempData
    {
        public int Length;
        public int NameLength;
        public int AlignLength;
        public int BraveLength;
        public int MovetypeLength;
        public int Capacity;
        public int NameCapacity;
        public int AlignCapacity;
        public int BraveCapacity;
        public int MovetypeCapacity;
        public RaceTree* Values;
        public RaceTree.NameAssignExpression* Names;
        public RaceTree.AlignAssignExpression* Aligns;
        public RaceTree.BraveAssignExpression* Braves;
        public RaceTree.MovetypeAssignExpression* Movetypes;
        public ListLinkedList Constis2;
        public IdentifierNumberPairListLinkedList IdentifierNumberPairs2;

        public RaceParserTempData(int capacity, Allocator allocator)
        {
            Length = NameLength = AlignLength = BraveLength = MovetypeLength = 0;
            Capacity = NameCapacity = AlignCapacity = BraveCapacity = MovetypeCapacity = capacity;
            IdentifierNumberPairs2 = new IdentifierNumberPairListLinkedList(capacity, allocator);
            if (capacity != 0)
            {
                Values = (RaceTree*)UnsafeUtility.Malloc(sizeof(RaceTree) * capacity, 4, allocator);
                Names = (RaceTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.NameAssignExpression) * capacity, 4, allocator);
                Aligns = (RaceTree.AlignAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.AlignAssignExpression) * capacity, 4, allocator);
                Braves = (RaceTree.BraveAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.BraveAssignExpression) * capacity, 4, allocator);
                Movetypes = (RaceTree.MovetypeAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.MovetypeAssignExpression) * capacity, 4, allocator);
                Constis2 = new ListLinkedList(capacity, sizeof(RaceTree.ConstiAssignExpression), allocator);
            }
            else
            {
                Values = null;
                Names = null;
                Aligns = null;
                Braves = null;
                Movetypes = null;
                Constis2 = default;
            }
        }

        public void Dispose(Allocator allocator)
        {
            if (NameCapacity != 0)
                UnsafeUtility.Free(Names, allocator);
            if (AlignCapacity != 0)
                UnsafeUtility.Free(Aligns, allocator);
            if (BraveCapacity != 0)
                UnsafeUtility.Free(Braves, allocator);
            if (MovetypeCapacity != 0)
                UnsafeUtility.Free(Movetypes, allocator);
            if (Capacity != 0)
                UnsafeUtility.Free(Values, allocator);
            IdentifierNumberPairs2.Dispose(allocator);
            Constis2.Dispose(allocator);
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
                    throw new System.Exception();
                case PendingReason.SectionListCapacityShortage:
                    switch ((RaceTree.Kind)result.SubDataIndex)
                    {
                        case RaceTree.Kind.name: // name
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " name lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Names, ref NameCapacity, allocator);
                            break;
                        case RaceTree.Kind.align: // align
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " align lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Aligns, ref AlignCapacity, allocator);
                            break;
                        case RaceTree.Kind.brave: // brave
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " brave lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Braves, ref BraveCapacity, allocator);
                            break;
                        case RaceTree.Kind.consti: //consti
                            throw new System.Exception();
                        case RaceTree.Kind.movetype: // movetype
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " movetype lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Movetypes, ref MovetypeCapacity, allocator);
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