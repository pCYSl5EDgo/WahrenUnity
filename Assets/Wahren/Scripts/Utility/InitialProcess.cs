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
                ListUtility.Lengthen(ref @this.Values, ref @this.Capacity, Allocator.Temp);
            }
            @this.Values[@this.Length++] = ast;
        }
    }
}