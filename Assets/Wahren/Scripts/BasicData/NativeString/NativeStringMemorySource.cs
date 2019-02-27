using System.Runtime.InteropServices;
using Unity.Collections;
using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;

namespace pcysl5edgo.Wahren.AST
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NativeStringMemorySource : ILinkedListNode<ushort, NativeStringMemorySource>
    {
        [FieldOffset(0)]
        public ListLinkedListNode Node;
        [FieldOffset(0)]
        public _ This;

        public NativeStringMemorySource* Next { get => This.Next; set => This.Next = value; }

        public bool IsFull => This.Length == This.Capacity;

        public struct _
        {
            public NativeStringMemorySource* Next;
            public ushort* Values;
            public int Capacity;
            public int Length;

        }

        public NativeStringMemorySource(int capacity, Allocator allocator)
        {
            this = default;
            Node = new ListLinkedListNode(capacity, sizeof(ushort), allocator);
        }

        public void Dispose(Allocator allocator) => Node.Dispose(allocator);

        public ref ushort GetRef(int index) => ref This.Values[index];

        public ushort* GetPointer(int index) => This.Values + index;

        public bool TryAdd(ushort* values, int length, out int start) => Node.TryAdd(values, length, out start);
    }
}