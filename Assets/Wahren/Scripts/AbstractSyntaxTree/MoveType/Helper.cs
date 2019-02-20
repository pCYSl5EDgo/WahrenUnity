namespace pcysl5edgo.Wahren.AST
{
    internal static class MovetypeHelper
    {
        public static TryInterpretReturnValue CreatePending(this MovetypeTree.Kind kind, Span span)
        => TryInterpretReturnValue.CreatePending(span, Location.Movetype, PendingReason.SectionListCapacityShortage, (int)kind);
        public static ASTValueTypePair CreateASTPair(this MovetypeTree.Kind kind)
        => new ASTValueTypePair((int)kind);
    }
}