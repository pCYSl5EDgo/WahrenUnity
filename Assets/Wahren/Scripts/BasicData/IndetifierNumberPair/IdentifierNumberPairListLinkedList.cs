using System.Runtime.InteropServices;
using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct IdentifierNumberPairListLinkedList : ILinkedList<IdentifierNumberPair, IdentifierNumberPairList>
    {
        [FieldOffset(0)]
        public ListLinkedList List;
        [FieldOffset(0)]
        public volatile IdentifierNumberPairList* First;
        [FieldOffset(8)]
        public volatile int NodeCapacity;

        public IdentifierNumberPairListLinkedList(int capacity, Allocator allocator)
        {
            this = default;
            List = new ListLinkedList(capacity, sizeof(IdentifierNumberPair), allocator);
        }

        public void Dispose(Allocator allocator) => List.Dispose(allocator);

        public void AddRange(IdentifierNumberPair* values, int length, out IdentifierNumberPairList* page, out int start, Allocator allocator)
        {
            List.AddRange(values, length, out var _page, out start, allocator);
            page = (IdentifierNumberPairList*)_page;
        }
    }
}