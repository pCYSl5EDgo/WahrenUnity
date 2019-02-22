using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct RaceParserTempData : IParserTempData
    {
        internal struct OldLengths
        {
#pragma warning disable CS0649
            public int Length;
            public int NameLength;
            public int AlignLength;
            public int BraveLength;
            public int ConstiLength;
            public int MoveTypeLength;
#pragma warning restore

            public static bool IsChanged(OldLengths* left, RaceParserTempData* right)
            => UnsafeUtility.MemCmp(left, right, sizeof(OldLengths)) == 0;
        }
        public int Length;
        public int NameLength;
        public int AlignLength;
        public int BraveLength;
        public int ConstiLength;
        public int MovetypeLength;
        public int Capacity;
        public int NameCapacity;
        public int AlignCapacity;
        public int BraveCapacity;
        public int ConstiCapacity;
        public int MovetypeCapacity;
        public RaceTree* Values;
        public RaceTree.NameAssignExpression* Names;
        public RaceTree.AlignAssignExpression* Aligns;
        public RaceTree.BraveAssignExpression* Braves;
        public RaceTree.ConstiAssignExpression* Constis;
        public RaceTree.MovetypeAssignExpression* Movetypes;
        public IdentifierNumberPairList IdentifierNumberPairs;

        public RaceParserTempData(int capacity, Allocator allocator)
        {
            Length = NameLength = AlignLength = BraveLength = ConstiLength = MovetypeLength = 0;
            Capacity = NameCapacity = AlignCapacity = BraveCapacity = ConstiCapacity = MovetypeCapacity = capacity;
            IdentifierNumberPairs = new IdentifierNumberPairList(capacity, allocator);
            if (capacity != 0)
            {
                Values = (RaceTree*)UnsafeUtility.Malloc(sizeof(RaceTree) * capacity, 4, allocator);
                Names = (RaceTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.NameAssignExpression) * capacity, 4, allocator);
                Aligns = (RaceTree.AlignAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.AlignAssignExpression) * capacity, 4, allocator);
                Braves = (RaceTree.BraveAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.BraveAssignExpression) * capacity, 4, allocator);
                Constis = (RaceTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.ConstiAssignExpression) * capacity, 4, allocator);
                Movetypes = (RaceTree.MovetypeAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.MovetypeAssignExpression) * capacity, 4, allocator);
            }
            else
            {
                Values = null;
                Names = null;
                Aligns = null;
                Braves = null;
                Constis = null;
                Movetypes = null;
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
            if (ConstiCapacity != 0)
                UnsafeUtility.Free(Constis, allocator);
            if (Capacity != 0)
                UnsafeUtility.Free(Values, allocator);
            IdentifierNumberPairs.Dispose(allocator);
            this = default;
        }

        public void Lengthen(ref ASTTypePageIndexPairList astValueTypePairList, in TryInterpretReturnValue result
#if UNITY_EDITOR
        , bool ShowLog
#endif
        )
        {
            ref var identifierNumberPairs = ref IdentifierNumberPairs;
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
                    ListUtility.Lengthen(ref astValueTypePairList.Values, ref astValueTypePairList.Capacity);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
#if UNITY_EDITOR
                    if (ShowLog)
                    {
                        UnityEngine.Debug.Log(prefix + " identifier number pair lengthen\n" + result.ToString() + "\nCapacity: " + identifierNumberPairs.Capacity + " , Length: " + identifierNumberPairs.Length);
                    }
#endif
                    identifierNumberPairs.Lengthen(Allocator.Persistent);
                    break;
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
                            ListUtility.Lengthen(ref Names, ref NameCapacity);
                            break;
                        case RaceTree.Kind.align: // align
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " align lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Aligns, ref AlignCapacity);
                            break;
                        case RaceTree.Kind.brave: // brave
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " brave lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Braves, ref BraveCapacity);
                            break;
                        case RaceTree.Kind.consti: //consti
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " consti lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Constis, ref ConstiCapacity);
                            break;
                        case RaceTree.Kind.movetype: // movetype
#if UNITY_EDITOR
                            if (ShowLog)
                            {
                                UnityEngine.Debug.Log(prefix + " movetype lengthen\n" + result.ToString());
                            }
#endif
                            ListUtility.Lengthen(ref Movetypes, ref MovetypeCapacity);
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