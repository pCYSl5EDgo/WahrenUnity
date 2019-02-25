namespace pcysl5edgo.Wahren.AST
{
    internal static class MovetypeHelper
    {
        public static ASTTypePageIndexPair CreateASTPair(this MovetypeTree.Kind kind)
        => new ASTTypePageIndexPair((int)kind);
    }
}