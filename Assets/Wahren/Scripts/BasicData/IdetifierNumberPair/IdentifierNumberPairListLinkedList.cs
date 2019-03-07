using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct IdentifierNumberPairListLinkedList : ILinkedList<IdentifierNumberPair, IdentifierNumberPairList>
    {
        public ListLinkedList List;

        public IdentifierNumberPairListLinkedList(int capacity, Allocator allocator)
        {
            this = default;
            List = new ListLinkedList(capacity, sizeof(IdentifierNumberPair), allocator);
        }

        public void Dispose(Allocator allocator) => List.Dispose(allocator);

        public void AddRange(IdentifierNumberPair* copySource, int length, out IdentifierNumberPairList* page, out int start, Allocator allocator) => List.AddRange(copySource, length, out page, out start, allocator);
    }
}