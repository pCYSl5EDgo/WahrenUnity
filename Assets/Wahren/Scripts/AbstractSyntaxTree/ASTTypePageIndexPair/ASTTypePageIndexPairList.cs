using System.Threading;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ASTTypePageIndexPairList : ILinkedListNode<ASTTypePageIndexPair, ASTTypePageIndexPairList>
    {
        [FieldOffset(0)]
        public ListLinkedListNode Node;
        [FieldOffset(0)]
        public _ This;
        public unsafe struct _
        {
            public ASTTypePageIndexPairList* NextNode;
            public ASTTypePageIndexPair* Values;
            public int Capacity;
            public int Length;
        }

        public ASTTypePageIndexPairList(int capacity, Allocator allocator)
        {
            this = default;
            if (capacity == 0)
            {
                return;
            }
            Node = new ListLinkedListNode(capacity, sizeof(ASTTypePageIndexPair), allocator);
        }

        public ref ASTTypePageIndexPair this[int index] => ref GetRef(index);

        public ref ASTTypePageIndexPair GetRef(int index) => ref This.Values[index];
        public ASTTypePageIndexPair* GetPointer(int index) => This.Values + index;
        public ASTTypePageIndexPairList* Next { get => This.NextNode; set => This.NextNode = value; }

        public bool TryAdd(ASTTypePageIndexPair* values, int length, out int start) => Node.TryAdd(values, length, out start);

        public int TryAddBulkMultiThread(in ASTTypePageIndexPairList list, out int length)
        {
            if (list.This.Length == 0)
            {
                length = 0;
                return 0;
            }
            int start = -1;
            length = list.This.Length;
            do
            {
                start = This.Length;
                if (start + length > This.Capacity)
                {
                    length = 0;
                    return -1;
                }
            } while (start != Interlocked.CompareExchange(ref This.Length, start + length, start));
            UnsafeUtility.MemCpy(This.Values + start, list.This.Values, sizeof(ASTTypePageIndexPair) * length);
            return start;
        }

        public void Dispose(Allocator allocator)
        {
            if (This.NextNode != null)
                This.NextNode->Dispose(allocator);
            if (This.Capacity != 0)
                UnsafeUtility.Free(This.Values, allocator);
            this = default;
        }
    }
}
