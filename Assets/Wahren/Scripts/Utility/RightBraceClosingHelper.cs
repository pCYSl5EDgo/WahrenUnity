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
        public static unsafe TryInterpretReturnValue CloseBrace<TTree>(this ref TTree tree, ref int treeStart, out int treeLength, ref ASTValueTypePairList astValueTypePairList, in ASTValueTypePairList tmpList, ref TTree* dataValues, int dataCapacity, ref int dataLength, Location location, int successSentence, ref int treeIndex, Caret nextToLeftBrace, ref Caret nextToRightBrace)
        where TTree : unmanaged, INameStruct
        {
            nextToRightBrace.Column++;
            treeStart = astValueTypePairList.TryAddBulkMultiThread(tmpList, out treeLength);
            if (treeStart == -1)
            {
                return TryInterpretReturnValue.CreatePending(new Span(nextToLeftBrace, 0), Location.Race, PendingReason.ASTValueTypePairListCapacityShortage);
            }
            if (tree.TryAddToMultiThread(ref dataValues, dataCapacity, ref dataLength, out treeIndex))
            {
                return new TryInterpretReturnValue(nextToRightBrace, SuccessSentence.RaceTreeIntrepretSuccess, InterpreterStatus.Success);
            }
            return TryInterpretReturnValue.CreatePending(new Span(nextToLeftBrace, 0), Location.Race, PendingReason.TreeListCapacityShortage);
        }
    }
}
