﻿using Unity.Collections;

namespace pcysl5edgo.Wahren.AST
{
    internal unsafe ref struct InitialProc_USING_STRUCT
    {
        public ASTTypePageIndexPairList list;
        public Allocator allocator;
        public InitialProc_USING_STRUCT(int capacity, in TextFile file, in Caret left, out Caret right, out TryInterpretReturnValue answer, Allocator allocator)
        {
            this.allocator = allocator;
            list = new ASTTypePageIndexPairList(capacity, Allocator.Temp);
            right = left;
            file.SkipWhiteSpace(ref right);
            answer = new TryInterpretReturnValue
            {
                Span = new Span { File = file.FilePathId },
            };
        }
        public void Dispose()
        {
            list.Dispose(Allocator.Temp);
            this = default;
        }

        public void Add(ASTTypePageIndexPair ast)
        {
            ref var @this = ref list.This;
            if (list.IsFull)
            {
                ListUtility.Lengthen(ref @this.Values, ref @this.Capacity, Allocator.Temp);
            }
            @this.Values[@this.Length++] = ast;
        }
    }
}