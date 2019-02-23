using System.Runtime.InteropServices;
using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ASTTypePageIndexPiarListLinkedList : ILinkedList<ASTTypePageIndexPair, ASTTypePageIndexPairList>
    {
        [FieldOffset(0)]
        public ListLinkedList List;
        [FieldOffset(0)]
        public _ This;
        public struct _
        {
            public volatile ASTTypePageIndexPairList* First;
            public volatile int NodeCapacity;
        }


        public ASTTypePageIndexPiarListLinkedList(int capacity, Allocator allocator)
        {
            this = default;
            List = new ListLinkedList(capacity, sizeof(ASTTypePageIndexPair), allocator);
        }

        public void Dispose(Allocator allocator) => List.Dispose(allocator);

        public void AddRange(ASTTypePageIndexPair* values, int length, out ASTTypePageIndexPairList* page, out int start, Allocator allocator)
        {
            List.AddRange(values, length, out var _page, out start, allocator);
            page = (ASTTypePageIndexPairList*)_page;
        }
    }
}