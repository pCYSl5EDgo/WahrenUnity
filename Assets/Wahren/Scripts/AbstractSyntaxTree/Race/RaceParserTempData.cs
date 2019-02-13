using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct RaceParserTempData : IDisposable
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
        public int MoveTypeLength;
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
        public RaceTree.MoveTypeAssignExpression* MoveTypes;
        public IdentifierNumberPairList IdentifierNumberPairs;

        public RaceParserTempData(int capacity)
        {
            Length = NameLength = AlignLength = BraveLength = ConstiLength = MoveTypeLength = 0;
            Capacity = NameCapacity = AlignCapacity = BraveCapacity = ConstiCapacity = MoveTypeCapacity = capacity;
            IdentifierNumberPairs = new IdentifierNumberPairList(capacity);
            if (capacity != 0)
            {
                Values = (RaceTree*)UnsafeUtility.Malloc(sizeof(RaceTree) * capacity, 4, Allocator.Persistent);
                Names = (RaceTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.NameAssignExpression) * capacity, 4, Allocator.Persistent);
                Aligns = (RaceTree.AlignAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.AlignAssignExpression) * capacity, 4, Allocator.Persistent);
                Braves = (RaceTree.BraveAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.BraveAssignExpression) * capacity, 4, Allocator.Persistent);
                Constis = (RaceTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.ConstiAssignExpression) * capacity, 4, Allocator.Persistent);
                MoveTypes = (RaceTree.MoveTypeAssignExpression*)UnsafeUtility.Malloc(sizeof(RaceTree.MoveTypeAssignExpression) * capacity, 4, Allocator.Persistent);
            }
            else
            {
                Values = null;
                Names = null;
                Aligns = null;
                Braves = null;
                Constis = null;
                MoveTypes = null;
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
                UnsafeUtility.Free(MoveTypes, Allocator.Persistent);
            if (ConstiCapacity != 0)
                UnsafeUtility.Free(Constis, Allocator.Persistent);
            if (Capacity != 0)
                UnsafeUtility.Free(Values, Allocator.Persistent);
            IdentifierNumberPairs.Dispose();
            this = default;
        }
    }
}