using System;
using System.Threading;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe interface ILinkedList<TValue, TNode>
        where TValue : unmanaged
        where TNode : unmanaged, ILinkedListNode<TValue, TNode>
    {
        void Dispose(Allocator allocator);
        void AddRange(TValue* values, int length, out TNode* page, out int start, Allocator allocator);
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ListLinkedList : ILinkedList<byte, ListLinkedListNode>
    {
        [FieldOffset(0)]
        public volatile ListLinkedListNode* First;
        [FieldOffset(8)]
        public volatile int NodeCapacity;

        public ListLinkedList(int capacity, Allocator allocator)
        {
            if (capacity < 1) throw new ArgumentOutOfRangeException(capacity.ToString() + " must be greater than 0");
            First = (ListLinkedListNode*)UnsafeUtility.Malloc(sizeof(ListLinkedListNode), 4, allocator);
            UnsafeUtility.MemClear(First, sizeof(ListLinkedListNode));
            NodeCapacity = capacity;
        }

        public void Dispose(Allocator allocator)
        {
            if (First != null)
            {
                First->Dispose(allocator);
                UnsafeUtility.Free(First, allocator);
            }
            this = default;
        }
        public void AddRange(byte* values, int length, out ListLinkedListNode* page, out int index, Allocator allocator) => AddRange<byte>(values, length, out page, out index, allocator);
        public void Add<T>(T value, out ListLinkedListNode* page, out int index, Allocator allocator) where T : unmanaged
        {
            var tryNode = First;
            while (true)
            {
                if (tryNode->TryAdd(&value, 1, out index))
                {
                    page = tryNode;
                    return;
                }
                if (tryNode->NextNodePtr == IntPtr.Zero)
                {
                    (page = ListLinkedListNode.Create<T>(NodeCapacity, allocator))->GetRef<T>(index = 0) = value;
                    while (IntPtr.Zero != Interlocked.CompareExchange(ref tryNode->NextNodePtr, new IntPtr(page), IntPtr.Zero))
                    {
                        tryNode = tryNode->Next;
                    }
                    return;
                }
                tryNode = tryNode->Next;
            }
        }
        public void AddRange<T>(T* values, int length, out ListLinkedListNode* page, out int start, Allocator allocator) where T : unmanaged
        {
            var tryNode = First;
            if (length > NodeCapacity)
            {
                NodeCapacity = length;
                goto ADDNEWPAGE;
            }
            while (true)
            {
                if (tryNode->TryAdd(values, length, out start))
                {
                    page = tryNode;
                    return;
                }
                if (tryNode->NextNodePtr == IntPtr.Zero)
                {
                    goto ADDNEWPAGE;
                }
                tryNode = tryNode->Next;
            }
        ADDNEWPAGE:
            page = ListLinkedListNode.Create<T>(NodeCapacity, allocator);
            start = 0;
            UnsafeUtility.MemCpy(page->Values, values, sizeof(T) * length);
            while (IntPtr.Zero != Interlocked.CompareExchange(ref tryNode->NextNodePtr, new IntPtr(page), IntPtr.Zero))
            {
                tryNode = tryNode->Next;
            }
        }
    }
}