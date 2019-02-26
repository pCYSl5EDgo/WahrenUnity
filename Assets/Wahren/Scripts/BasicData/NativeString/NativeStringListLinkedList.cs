using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct NativeStringListLinkedList
    {
        public ListLinkedList List;
        public NativeStringListLinkedList(int capacity, Allocator allocator)
        {
            this = default;
            List = new ListLinkedList(capacity, sizeof(IdentifierNumberPair), allocator);
        }

        public void Dispose(Allocator allocator) => List.Dispose(allocator);

        public void AddRange(NativeString* values, int length, out NativeStringList* page, out int start, Allocator allocator) => List.AddRange(values, length, out page, out start, allocator);
    }
}