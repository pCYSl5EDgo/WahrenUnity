﻿namespace pcysl5edgo.Wahren.AST
{
    internal static class RaceHelper
    {
        public static TryInterpretReturnValue CreatePending(this RaceTree.Kind kind, Span span)
        => TryInterpretReturnValue.CreatePending(span, Location.Race, PendingReason.SectionListCapacityShortage, (int)kind);
        public static ASTTypePageIndexPair CreateASTPair(this RaceTree.Kind kind)
        => new ASTTypePageIndexPair((int)kind);
    }
}