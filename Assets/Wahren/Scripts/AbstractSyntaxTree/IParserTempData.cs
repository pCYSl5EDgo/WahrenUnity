namespace pcysl5edgo.Wahren.AST
{
    public interface IParserTempData : System.IDisposable
    {
        void Lengthen(ref ASTValueTypePairList astValueTypePairList, in TryInterpretReturnValue result
#if UNITY_EDITOR
        , bool ShowLog
#endif
        );
    }
}