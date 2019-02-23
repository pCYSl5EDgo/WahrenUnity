using System.Threading;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct IdentifierNumberPairList : ILinkedListNode<IdentifierNumberPair, IdentifierNumberPairList>
    {
        [FieldOffset(0)]
        public ListLinkedListNode Node;
        [FieldOffset(0)]
        public _ This;

        public struct _
        {
            public IdentifierNumberPairList* Next;
            public IdentifierNumberPair* Values;
            public int Capacity;
            public int Length;
        }


        public IdentifierNumberPairList(int capacity, Allocator allocator)
        {
            this = default;
            Node = new ListLinkedListNode(capacity, sizeof(IdentifierNumberPair), allocator);
        }

        public override string ToString() => "capacity : " + This.Capacity + "\nlength : " + This.Length;

        public void Lengthen(Allocator allocator)
        {
            if (This.Capacity == 0) return;
            int size = sizeof(IdentifierNumberPair) * This.Capacity;
            var _ = UnsafeUtility.Malloc(size * 2, 4, allocator);
            UnsafeUtility.MemCpy(_, This.Values, size);
            UnsafeUtility.Free(This.Values, allocator);
            This.Values = (IdentifierNumberPair*)_;
            This.Capacity *= 2;
        }
        public void Dispose(Allocator allocator) => Node.Dispose(allocator);

        public ref IdentifierNumberPair GetRef(int index) => ref This.Values[index];

        public IdentifierNumberPair* GetPointer(int index) => This.Values + index;

        public bool TryAdd(IdentifierNumberPair* values, int length, out int start)
        {
            do
            {
                start = This.Length;
                if (start + length > This.Capacity)
                    return false;
            } while (start != Interlocked.CompareExchange(ref This.Length, start + length, start));
            UnsafeUtility.MemCpy(This.Values + start, values, sizeof(IdentifierNumberPair) * length);
            return true;
        }
        public IdentifierNumberPairList* Next { get => This.Next; set => This.Next = value; }
    }
}