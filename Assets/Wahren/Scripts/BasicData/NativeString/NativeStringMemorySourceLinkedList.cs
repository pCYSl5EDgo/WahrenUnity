using Unity.Collections;
using System.Collections.Generic;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct NativeStringMemorySoyrceLinkedList : ILinkedList<ushort, NativeStringMemorySource>
    {
        public ListLinkedList List;
        public NativeStringMemorySoyrceLinkedList(int capacity, Allocator allocator) => List = new ListLinkedList(capacity, sizeof(ushort), allocator);
        public void AddRange(ushort* copySource, int length, out NativeStringMemorySource* page, out int start, Allocator allocator) => List.AddRange(copySource, length, out page, out start, allocator);
        public void AddRange(ushort* copySource, int length, out ushort* returncopySource, Allocator allocator)
        {
            AddRange(copySource, length, out var page, out var start, allocator);
            returncopySource = page->GetPointer(start);
        }
        public void AddRange(ushort* copySource, int length, Allocator allocator) => AddRange(copySource, length, out _, out _, allocator);

        public void Dispose(Allocator allocator) => List.Dispose(allocator);
    }
}