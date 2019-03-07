using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe interface ILinkedList<TValue, TNode>
        where TValue : unmanaged
        where TNode : unmanaged, ILinkedListNode<TValue, TNode>
    {
        void Dispose(Allocator allocator);
        void AddRange(TValue* copySource, int length, out TNode* page, out int start, Allocator allocator);
    }

    public unsafe struct ListLinkedList
    {
        public ListLinkedListNode* First;
        public int NodeCapacity;
        private ListLinkedListNode* LastFull;

        public ref struct NodeEnumerator
        {
            public NodeEnumerator(ref ListLinkedList list)
            {
                Node = list.First;
                isNotFirst = false;
            }
            private ListLinkedListNode* Node;
            public ref ListLinkedListNode Current => ref *Node;
            private bool isNotFirst;
            public bool MoveNext()
            {
                if (isNotFirst)
                    return (Node = Node->Next) != null;
                else
                    return isNotFirst = true;
            }
            public void Dispose() => Node = null;
            public void Reset() => throw new System.NotImplementedException();
        }

        public NodeEnumerator GetNodeEnumerator() => new NodeEnumerator(ref this);

        public ref struct ElementEnumerator<T> where T : unmanaged
        {
            public ElementEnumerator(ref ListLinkedList list)
            {
                index = -1;
                Node = list.First;
            }
            internal int index;
            internal ListLinkedListNode* Node;
            public ref T Current
            {
                get
                {
                    if (index < 0 || Node == null) throw new System.InvalidOperationException();
                    return ref Node->GetRef<T>(index);
                }
            }
            public bool MoveNext()
            {
                if (Node == null)
                    return false;
                if (++index == Node->Length)
                {
                    index = 0;
                    Node = Node->Next;
                    if (Node == null)
                        return false;
                }
                return true;
            }
            public void Dispose() => Node = null;
            public void Reset() => throw new System.NotImplementedException();
        }

        public ElementEnumerator<T> GetElementEnumerator<T>() where T : unmanaged => new ElementEnumerator<T>(ref this);

        public int Length
        {
            get
            {
                int count = 0;
                for (var node = First; node != null; node = node->Next)
                {
                    count += node->Length;
                }
                return count;
            }
        }

        public ListLinkedList(int capacity, int size, Allocator allocator)
        {
            if (capacity < 1) throw new ArgumentOutOfRangeException(capacity.ToString() + " must be greater than 0");
            First = (ListLinkedListNode*)UnsafeUtility.Malloc(sizeof(ListLinkedListNode), 4, allocator);
            *First = new ListLinkedListNode(capacity, size, allocator);
            NodeCapacity = capacity;
            LastFull = null;
        }

        public ref T GetRef<T>(int index) where T : unmanaged
        {
            if (index < 0) throw new ArgumentOutOfRangeException();
            for (var node = First; node != null; node = node->Next)
            {
                if (index < node->Length)
                    return ref node->GetRef<T>(index);
                index -= node->Length;
            }
            throw new ArgumentOutOfRangeException();
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
        public void Add<T>(ref T copySource, out ListLinkedListNode* page, out int index, Allocator allocator) where T : unmanaged
        {
            ListLinkedListNode* tryNode;
            if (LastFull == null)
            {
                tryNode = First;
            }
            else
            {
                tryNode = LastFull->Next;
            }
            for (; tryNode != null; tryNode = tryNode->Next)
            {
                if (tryNode->TryAdd(ref copySource, out index))
                {
                    if (tryNode->IsFull)
                        LastFull = tryNode;
                    page = tryNode;
                    return;
                }
                UnityEngine.Debug.Log("LAST FULL RENEW");
                LastFull = tryNode;
            }
            page = ListLinkedListNode.Create<T>(NodeCapacity, allocator);
            index = 0;
            page->GetRef<T>(0) = copySource;
            page->Length = 1;
            for (tryNode = LastFull; IntPtr.Zero != Interlocked.CompareExchange(ref tryNode->NextNodePtr, new IntPtr(page), IntPtr.Zero); tryNode = tryNode->Next) ;
        }
        public void Add<TValue, TNode>(ref TValue copySource, out TNode* page, out int index, Allocator allocator)
            where TValue : unmanaged
            where TNode : unmanaged, ILinkedListNode<TValue, TNode>
        {
            Add(ref copySource, out var _page, out index, allocator);
            page = (TNode*)_page;
        }
        public void AddRange<T>(T* copySource, int length, out ListLinkedListNode* page, out int start, Allocator allocator) where T : unmanaged
        {
            ListLinkedListNode* tryNode;
            if (LastFull == null)
                tryNode = First;
            else if (LastFull->NextNodePtr == IntPtr.Zero)
            {
                tryNode = LastFull;
                goto ADDNEWPAGE;
            }
            else tryNode = LastFull->Next;
            if (length > NodeCapacity)
            {
                NodeCapacity = length;
                goto ADDNEWPAGE;
            }
            while (true)
            {
                if (tryNode->TryAdd(copySource, length, out start))
                {
                    if (tryNode->IsFull && LastFull->Next == tryNode)
                        LastFull = tryNode;
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
            UnsafeUtility.MemCpy(page->Values, copySource, sizeof(T) * length);
            while (IntPtr.Zero != Interlocked.CompareExchange(ref tryNode->NextNodePtr, new IntPtr(page), IntPtr.Zero))
            {
                tryNode = tryNode->Next;
            }
        }
        public void AddRange<TValue, TNode>(TValue* copySource, int length, out TNode* page, out int start, Allocator allocator)
            where TValue : unmanaged
            where TNode : unmanaged
        {
            AddRange(copySource, length, out var _page, out start, allocator);
            page = (TNode*)_page;
        }
    }
}