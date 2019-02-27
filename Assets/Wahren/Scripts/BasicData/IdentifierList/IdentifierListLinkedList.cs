using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct IdentifierListLinkedList : ILinkedList<Span, IdentifierList>
    {
        public ListLinkedList List;

        public IdentifierListLinkedList(int capacity, Allocator allocator) => List = new ListLinkedList(capacity, sizeof(Span), allocator);

        public unsafe void AddRange(Span* values, int length, out IdentifierList* page, out int start, Allocator allocator) => List.AddRange(values, length, out page, out start, allocator);
        public void Dispose(Allocator allocator) => List.Dispose(allocator);
    }
}