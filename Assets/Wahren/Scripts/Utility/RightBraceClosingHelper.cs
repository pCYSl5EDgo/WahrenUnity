using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcysl5edgo.Wahren.AST
{
    public static class RightBraceClosingHelper
    {
        public static unsafe TryInterpretReturnValue CloseBrace<TTree>(this ref TTree tree, ref int treeStart, out int treeLength, ref ASTTypePageIndexPairList astValueTypePairList, in ASTTypePageIndexPairList tmpList, ref TTree* dataValues, int dataCapacity, ref int dataLength, Location location, SuccessSentence.Kind successKind, Caret nextToLeftBrace, ref Caret nextToRightBrace)
        where TTree : unmanaged, INameStruct
        {
            nextToRightBrace.Column++;
            treeStart = astValueTypePairList.TryAddBulkMultiThread(tmpList, out treeLength);
            if (treeStart == -1)
            {
                return TryInterpretReturnValue.CreatePending(new Span(nextToLeftBrace, 0), Location.Race, PendingReason.ASTValueTypePairListCapacityShortage);
            }
            if (tree.TryAddToMultiThread(ref dataValues, dataCapacity, ref dataLength, out _))
            {
                return new TryInterpretReturnValue(nextToRightBrace, successKind);
            }
            return TryInterpretReturnValue.CreatePending(new Span(nextToLeftBrace, 0), Location.Race, PendingReason.TreeListCapacityShortage);
        }
    }
}
