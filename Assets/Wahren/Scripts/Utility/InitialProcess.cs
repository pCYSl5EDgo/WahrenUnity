using Unity.Collections;

using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;

namespace pcysl5edgo.Wahren.AST
{
    internal unsafe struct InitialProc_USING_STRUCT
    {
        public ASTTypePageIndexPairList list;
        public InitialProc_USING_STRUCT(int capacity, in TextFile file, in Caret left, out Caret right, out TryInterpretReturnValue answer, out int treeIndex)
        {
            list = new ASTTypePageIndexPairList(capacity, Allocator.Temp);
            treeIndex = -1;
            right = left;
            file.SkipWhiteSpace(ref right);
            answer = new TryInterpretReturnValue
            {
                Span = new Span { File = file.FilePathId },
            };
        }
        public void Dispose()
        {
            list.Dispose(Allocator.Temp);
            this = default;
        }

        public void Add(ASTTypePageIndexPair ast)
        {
            ref var @this = ref list.This;
            if (list.IsFull)
            {
                long size = sizeof(ASTTypePageIndexPair) * @this.Capacity;
                var t = Malloc(size * 2, 4, Allocator.Temp);
                ref var node = ref list.Node;
                MemCpy(t, node.Values, size);
                @this.Capacity *= 2;
                Free(node.Values, Allocator.Temp);
                node.Values = t;
            }
            @this.Values[@this.Length++] = ast;
        }
    }
}