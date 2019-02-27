using Unity.Collections;
using System.Collections.Generic;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct NativeStringMemorySoyrceLinkedList : ILinkedList<ushort, NativeStringMemorySource>
    {
        public ListLinkedList List;
        public NativeStringMemorySoyrceLinkedList(int capacity, Allocator allocator) => List = new ListLinkedList(capacity, sizeof(ushort), allocator);
        public void AddRange(ushort* values, int length, out NativeStringMemorySource* page, out int start, Allocator allocator) => List.AddRange(values, length, out page, out start, allocator);
        public void AddRange(ushort* values, int length, out ushort* returnValues, Allocator allocator)
        {
            AddRange(values, length, out var page, out var start, allocator);
            returnValues = page->GetPointer(start);
        }

        public void Dispose(Allocator allocator) => List.Dispose(allocator);
    }
}