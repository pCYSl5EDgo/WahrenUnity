using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct RaceParserTempData
    {
        public ListLinkedList Values;
        public ListLinkedList Names;
        public ListLinkedList Aligns;
        public ListLinkedList Braves;
        public ListLinkedList Movetypes;
        public ListLinkedList Constis;
        public IdentifierNumberPairListLinkedList IdentifierNumberPairs;

        public RaceParserTempData(int capacity, Allocator allocator)
        {
            this = default;
            if (capacity != 0)
            {
                IdentifierNumberPairs = new IdentifierNumberPairListLinkedList(capacity, allocator);
                Values = new ListLinkedList(capacity, sizeof(RaceTree), allocator);
                Names = new ListLinkedList(capacity, sizeof(RaceTree.NameAssignExpression), allocator);
                Aligns = new ListLinkedList(capacity, sizeof(RaceTree.AlignAssignExpression), allocator);
                Braves = new ListLinkedList(capacity, sizeof(RaceTree.BraveAssignExpression), allocator);
                Movetypes = new ListLinkedList(capacity, sizeof(RaceTree.MovetypeAssignExpression), allocator);
                Constis = new ListLinkedList(capacity, sizeof(RaceTree.ConstiAssignExpression), allocator);
            }
        }

        public void Dispose(Allocator allocator)
        {
            Values.Dispose(allocator);
            IdentifierNumberPairs.Dispose(allocator);
            Names.Dispose(allocator);
            Aligns.Dispose(allocator);
            Braves.Dispose(allocator);
            Movetypes.Dispose(allocator);
            Constis.Dispose(allocator);
            this = default;
        }
    }
}