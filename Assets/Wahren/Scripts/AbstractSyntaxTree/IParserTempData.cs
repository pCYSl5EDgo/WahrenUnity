using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    public interface IParserTempData
    {
#if UNITY_EDITOR
        void Lengthen(ref ASTTypePageIndexPairList astValueTypePairList, in TryInterpretReturnValue result, Allocator allocator, bool ShowLog);
#else
        void Lengthen(ref ASTTypePageIndexPairList astValueTypePairList, in TryInterpretReturnValue result, Allocator allocator);
#endif
    }
}