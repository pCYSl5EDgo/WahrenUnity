using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe interface ILinkedListNode<TValue, TNode>
        where TValue : unmanaged
        where TNode : unmanaged
    {
        ref TValue GetRef(int index);
        TValue* GetPointer(int index);
        TNode* Next { get; set; }
        bool TryAdd(TValue* values, int length, out int start);
        void Dispose(Allocator allocator);
        bool IsFull { get; }
    }
    public unsafe struct ListLinkedListNode
    {
        public IntPtr NextNodePtr;
        public void* Values;
        public int Capacity;
        public int Length;

        public ListLinkedListNode(void* values, int capacity)
        {
            NextNodePtr = IntPtr.Zero;
            Values = values;
            Capacity = capacity;
            Length = 0;
        }
        public ListLinkedListNode(int capacity, int size, Allocator allocator)
        {
            Capacity = capacity;
            Length = 0;
            NextNodePtr = IntPtr.Zero;
            Values = UnsafeUtility.Malloc(size * capacity, 4, allocator);
            UnsafeUtility.MemClear(Values, size);
        }

        public static ListLinkedListNode* Create<T>(int capacity, Allocator allocator) where T : unmanaged
        {
            var answer = (ListLinkedListNode*)UnsafeUtility.Malloc(sizeof(ListLinkedListNode), 4, allocator);
            *answer = new ListLinkedListNode(capacity, sizeof(T), allocator);
            return answer;
        }

        public ref T GetRef<T>(int index) where T : unmanaged => ref ((T*)Values)[index];
        public T* GetPointer<T>(int index) where T : unmanaged => (T*)Values + index;
        public ListLinkedListNode* Next
        {
            get => (ListLinkedListNode*)NextNodePtr.ToPointer();
            set => NextNodePtr = new IntPtr(value);
        }

        public void Dispose(Allocator allocator)
        {
            if (NextNodePtr != IntPtr.Zero)
            {
                var next = Next;
                next->Dispose(allocator);
                UnsafeUtility.Free(next, allocator);
            }
            if (Capacity != 0)
            {
                UnsafeUtility.Free(Values, allocator);
            }
            this = default;
        }

        public bool TryAdd<T>(T* values, int length, out int start) where T : unmanaged
        {
            do
            {
                start = Length;
                if (start + length > Capacity)
                    return false;
            } while (start != Interlocked.CompareExchange(ref Length, start + length, start));
            UnsafeUtility.MemCpy(GetPointer<T>(start), values, sizeof(T) * length);
            return true;
        }
        public bool TryAdd(byte* values, int length, out int start) => TryAdd<byte>(values, length, out start);
    }
}