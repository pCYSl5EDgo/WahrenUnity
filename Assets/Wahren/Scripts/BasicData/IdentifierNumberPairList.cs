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
        public IdentifierNumberPairList* NextNodePtr;
        [FieldOffset(8)]
        public IdentifierNumberPair* Values;
        [FieldOffset(16)]
        public int Capacity;
        [FieldOffset(20)]
        public int Length;


        public IdentifierNumberPairList(int capacity, Allocator allocator)
        {
            this = default;
            Capacity = capacity;
            if (Capacity == 0)
                Values = null;
            else
                Values = (IdentifierNumberPair*)UnsafeUtility.Malloc(sizeof(IdentifierNumberPair) * capacity, 4, allocator);
        }

        public override string ToString() => "capacity : " + Capacity + "\nlength : " + Length;

        public void Lengthen(Allocator allocator)
        {
            if (Capacity == 0) return;
            int size = sizeof(IdentifierNumberPair) * Capacity;
            var _ = UnsafeUtility.Malloc(size * 2, 4, allocator);
            UnsafeUtility.MemCpy(_, Values, size);
            UnsafeUtility.Free(Values, allocator);
            Values = (IdentifierNumberPair*)_;
            Capacity *= 2;
        }
        public void Dispose(Allocator allocator)
        {
            if (NextNodePtr != null)
            {
                NextNodePtr->Dispose(allocator);
                UnsafeUtility.Free(NextNodePtr, allocator);
            }
            if (Capacity != 0)
            {
                UnsafeUtility.Free(Values, allocator);
            }
            this = default;
        }

        public ref IdentifierNumberPair GetRef(int index) => ref Values[index];

        public IdentifierNumberPair* GetPointer(int index) => Values + index;

        public bool TryAdd(IdentifierNumberPair* values, int length, out int start)
        {
            do
            {
                start = Length;
                if (start + length > Capacity)
                    return false;
            } while (start != Interlocked.CompareExchange(ref Length, start + length, start));
            UnsafeUtility.MemCpy(Values + start, values, sizeof(IdentifierNumberPair) * length);
            return true;
        }
        public IdentifierNumberPairList* Next { get => NextNodePtr; set => NextNodePtr = value; }
    }
}