using System;
using System.Threading;
using System.Runtime.InteropServices;
using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NativeStringList : ILinkedListNode<NativeString, NativeStringList>
    {
        [FieldOffset(0)]
        public ListLinkedListNode Node;
        [FieldOffset(0)]
        public _ This;

        public NativeStringList* Next { get => This.Next; set => This.Next = value; }

        public bool IsFull => This.Length == This.Capacity;

        public NativeStringList(int capacity, Allocator allocator)
        {
            this = default;
            Node = new ListLinkedListNode(capacity, sizeof(NativeString), allocator);
        }
        public void Dispose(Allocator allocator) => Node.Dispose(allocator);

        public NativeString* GetPointer(int index) => Node.GetPointer<NativeString>(index);

        public ref NativeString GetRef(int index) => ref Node.GetRef<NativeString>(index);

        public bool TryAdd(NativeString* values, int length, out int start) => Node.TryAdd(values, length, out start);

        public struct _
        {
            public NativeStringList* Next;
            public NativeString* Values;
            public int Capacity;
            public int Length;
        }
    }
}