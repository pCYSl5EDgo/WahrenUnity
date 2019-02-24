using System.Threading;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct ASTTypePageIndexPair
    {
        public int Type;
        public ListLinkedListNode* Page;
        public int Index;

        public ASTTypePageIndexPair(int type)
        {
            this.Type = type;
            this.Page = null;
            this.Index = default;
        }

        public void Deconstruct(out int type, out ListLinkedListNode* page, out int index)
        {
            type = Type;
            page = Page;
            index = Index;
        }

        public ref T GetRef<T>() where T : unmanaged => ref Page->GetRef<T>(Index);

        public bool TryAddAST<T>(T* list, in T value, int capacity, ref int length) where T : unmanaged
        {
            do
            {
                if (capacity == length)
                {
                    return false;
                }
                Index = length;
            } while (Index != Interlocked.CompareExchange(ref length, Index + 1, Index));
            list[Index] = value;
            return true;
        }
    }
}