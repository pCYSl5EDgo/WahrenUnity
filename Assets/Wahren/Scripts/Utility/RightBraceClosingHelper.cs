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
        public static unsafe TryInterpretReturnValue CloseBrace<TTree>(this ref TTree tree, out ASTTypePageIndexPairList* treePage, out int treeStart, out int treeLength, ref ASTTypePageIndexPairListLinkedList astValueTypePairList, in ASTTypePageIndexPairList tmpList, ref ListLinkedList data, Location location, SuccessSentence.Kind successKind, Caret nextToLeftBrace, ref Caret nextToRightBrace, Allocator allocator) where TTree : unmanaged, INameStruct
        {
            nextToRightBrace.Column++;
            astValueTypePairList.AddRange(tmpList.This.Values, treeLength = tmpList.This.Length, out treePage, out treeStart, allocator);
            data.Add(ref tree, out _, out _, allocator);
            return new TryInterpretReturnValue(nextToRightBrace, successKind);
        }
    }
}
