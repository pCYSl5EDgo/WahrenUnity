namespace pcysl5edgo.Wahren.AST
{
    public interface IParserTempData
    {
        void Lengthen(ref ASTTypePageIndexPairList astValueTypePairList, in TryInterpretReturnValue result
#if UNITY_EDITOR
        , bool ShowLog
#endif
        );
    }
}