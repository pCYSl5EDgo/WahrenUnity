namespace pcysl5edgo.Wahren.AST
{
    internal static class RaceHelper
    {
        public static ASTTypePageIndexPair CreateASTPair(this RaceTree.Kind kind)
        => new ASTTypePageIndexPair((int)kind);
    }
}