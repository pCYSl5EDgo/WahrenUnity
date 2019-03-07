using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct NativeStringListLinkedList : ILinkedList<NativeString, NativeStringList>
    {
        public ListLinkedList List;
        public NativeStringListLinkedList(int capacity, Allocator allocator) => List = new ListLinkedList(capacity, sizeof(NativeString), allocator);
        public void AddRange(NativeString* copySource, int length, out NativeStringList* page, out int start, Allocator allocator) => List.AddRange(copySource, length, out page, out start, allocator);
        public void Add(ref NativeString copySource, out NativeStringList* page, out int start, Allocator allocator) => List.Add(ref copySource, out page, out start, allocator);
        public void Dispose(Allocator allocator) => List.Dispose(allocator);
    }
}