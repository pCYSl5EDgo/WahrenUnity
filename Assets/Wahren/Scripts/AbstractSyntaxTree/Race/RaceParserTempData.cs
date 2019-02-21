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
        public int MoveTypeCapacity;
        public RaceTree* Values;
        public RaceTree.NameAssignExpression* Names;
        public RaceTree.AlignAssignExpression* Aligns;
        public RaceTree.BraveAssignExpression* Braves;
        public RaceTree.ConstiAssignExpression* Constis;
        public RaceTree.MoveTypeAssignExpression* Movetypes;
        public IdentifierNumberPairList IdentifierNumberPairs;

        public RaceParserTempData(int capacity)
        {
            Length = NameLength = AlignLength = BraveLength = ConstiLength = MovetypeLength = 0;
            Capacity = NameCapacity = AlignCapacity = BraveCapacity = ConstiCapacity = MoveTypeCapacity = capacity;
            IdentifierNumberPairs = new IdentifierNumberPairList(capacity);
            if (capacity != 0)
            {
                Values = (RaceTree*)UnsafeUtility.Malloc(sizeof(RaceTree) * capacity, 4, Allocator.Persistent);
                Names = (RaceTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.NameAssignExpression) * capacity, 4, Allocator.Persistent);
                Aligns = (RaceTree.AlignAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.AlignAssignExpression) * capacity, 4, Allocator.Persistent);
                Braves = (RaceTree.BraveAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.BraveAssignExpression) * capacity, 4, Allocator.Persistent);
                Constis = (RaceTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.ConstiAssignExpression) * capacity, 4, Allocator.Persistent);
                Movetypes = (RaceTree.MoveTypeAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.MoveTypeAssignExpression) * capacity, 4, Allocator.Persistent);
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

        public void Dispose()
        {
            if (NameCapacity != 0)
                UnsafeUtility.Free(Names, Allocator.Persistent);
            if (AlignCapacity != 0)
                UnsafeUtility.Free(Aligns, Allocator.Persistent);
            if (BraveCapacity != 0)
                UnsafeUtility.Free(Braves, Allocator.Persistent);
            if (MoveTypeCapacity != 0)
                UnsafeUtility.Free(Movetypes, Allocator.Persistent);
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
                    identifierNumberPairs.Lengthen();
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
                            ListUtility.Lengthen(ref Movetypes, ref MoveTypeCapacity);
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