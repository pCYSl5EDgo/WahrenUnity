using System.Runtime.InteropServices;
using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct IdentifierList : ILinkedListNode<Span, IdentifierList>
    {
        [FieldOffset(0)]
        public ListLinkedListNode Node;
        [FieldOffset(0)]
        public _ This;

        public IdentifierList* Next { get => This.Next; set => This.Next = value; }

        public bool IsFull => This.Capacity == This.Length;

        public IdentifierList(int capacity, Allocator allocator)
        {
            this = default;
            Node = new ListLinkedListNode(capacity, sizeof(Span), allocator);
        }

        public void Dispose(Allocator allocator) => Node.Dispose(allocator);

        public Span* GetPointer(int index) => This.Values + index;

        public ref Span GetRef(int index) => ref This.Values[index];

        public bool TryAdd(Span* values, int length, out int start) => Node.TryAdd(values, length, out start);

        public struct _
        {
            public IdentifierList* Next;
            public Span* Values;
            public int Capacity;
            public int Length;
        }
    }
}