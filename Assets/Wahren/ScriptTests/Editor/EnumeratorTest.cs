using Unity.Collections;
using NUnit.Framework;
using pcysl5edgo.Wahren.AST;


using static UnityEngine.Debug;
using static UnityEngine.Assertions.Assert;

namespace Tests
{
    public unsafe class EnumeratorTest
    {
        private const Allocator PersistentAllocator = Allocator.Persistent;

        [Test]
        public void ListLinkedListTest()
        {
            const int loopCount = 32;
            const int nodeCapa = 2;
            var list = new ListLinkedList(nodeCapa, sizeof(ushort), PersistentAllocator);
            try
            {
                AreEqual(list.NodeCapacity, nodeCapa);
                for (ushort i = 0; i < loopCount; i++)
                {
                    list.Add(ref i, out var page, out var index, PersistentAllocator);
                    IsFalse(page == null);
                    AreEqual(page->GetRef<ushort>(index), i);
                }
                AreEqual(list.Length, loopCount);
                var elementEnumerator = list.GetElementEnumerator<ushort>();
                AreEqual(elementEnumerator.index, -1);
                int count = 0;
                while (elementEnumerator.MoveNext())
                {
                    AreEqual(elementEnumerator.Current, count);
                    AreEqual(elementEnumerator.Current, count);
                    AreEqual(elementEnumerator.Current, count);
                    AreEqual(elementEnumerator.Current, count);
                    AreEqual(elementEnumerator.Current, count++);
                }
                AreEqual(count, loopCount);
            }
            finally
            {
                list.Dispose(PersistentAllocator);
            }
        }
    }
}
