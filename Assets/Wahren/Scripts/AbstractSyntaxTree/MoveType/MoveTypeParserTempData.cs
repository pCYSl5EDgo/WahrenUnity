using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct MovetypeParserTempData : System.IDisposable
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

        public MoveTypeTree* Values;
        public MoveTypeTree.NameAssignExpression* Names;
        public MoveTypeTree.HelpAssignExpression* Helps;
        public MoveTypeTree.ConstiAssignExpression* Constis;
        public IdentifierNumberPairList IdentifierNumberPairs;

        public MovetypeParserTempData(int capacity)
        {
            Length = NameLength = HelpLength = ConstiLength = 0;
            Capacity = NameCapacity = HelpCapacity = ConstiCapacity = capacity;
            IdentifierNumberPairs = new IdentifierNumberPairList(capacity);
            if (capacity != 0)
            {
                Values = (MoveTypeTree*)UnsafeUtility.Malloc(sizeof(MoveTypeTree) * capacity, 4, Allocator.Persistent);
                Names = (MoveTypeTree.NameAssignExpression*)UnsafeUtility.Malloc(sizeof(MoveTypeTree.NameAssignExpression) * capacity, 4, Allocator.Persistent);
                Helps = (MoveTypeTree.HelpAssignExpression*)UnsafeUtility.Malloc(sizeof(MoveTypeTree.HelpAssignExpression) * capacity, 4, Allocator.Persistent);
                Constis = (MoveTypeTree.ConstiAssignExpression*)UnsafeUtility.Malloc(sizeof(MoveTypeTree.ConstiAssignExpression) * capacity, 4, Allocator.Persistent);
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
    }
}