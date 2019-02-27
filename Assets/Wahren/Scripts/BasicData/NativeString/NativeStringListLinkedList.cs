using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct NativeStringListLinkedList : ILinkedList<NativeString, NativeStringList>
    {
        public ListLinkedList List;
        public NativeStringListLinkedList(int capacity, Allocator allocator) => List = new ListLinkedList(capacity, sizeof(NativeString), allocator);
        public void AddRange(NativeString* values, int length, out NativeStringList* page, out int start, Allocator allocator) => List.AddRange(values, length, out page, out start, allocator);
        public void Add(ref NativeString value, out NativeStringList* page, out int start, Allocator allocator) => List.Add(ref value, out page, out start, allocator);
        public void Dispose(Allocator allocator) => List.Dispose(allocator);
    }
}