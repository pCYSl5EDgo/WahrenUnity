namespace pcysl5edgo.Wahren.AST
{
    internal static class VoiceHelper
    {
        public static ASTTypePageIndexPair CreateASTPair(this VoiceTree.Kind kind)
        => new ASTTypePageIndexPair((int)kind);
    }
}