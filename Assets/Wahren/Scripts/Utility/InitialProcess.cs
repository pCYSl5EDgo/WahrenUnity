using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    internal struct InitialProc_USING_STRUCT : System.IDisposable
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
    }
}