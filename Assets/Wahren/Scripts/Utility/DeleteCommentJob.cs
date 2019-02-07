using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    [BurstCompile]
    public unsafe struct DeleteCommentJob : IJob
    {
        public byte isDebug;
        [NativeDisableUnsafePtrRestriction] public int* LineCountPtr;
        [NativeDisableUnsafePtrRestriction] public ushort* Contents;
        [NativeDisableUnsafePtrRestriction] public int* LineStarts;
        [NativeDisableUnsafePtrRestriction] public int* LineLengths;

        public static JobHandle Schedule(TextFile* filePtr, bool isDebug)
        => new DeleteCommentJob(filePtr, isDebug).Schedule();

        public DeleteCommentJob(TextFile* filePtr, bool isDebug)
        {
            this.Contents = filePtr->Contents;
            this.LineStarts = filePtr->LineStarts;
            this.LineLengths = filePtr->LineLengths;
            if (isDebug)
                this.isDebug = 1;
            else
                this.isDebug = 0;
            this.LineCountPtr = &filePtr->LineCount;
        }

        public void Execute()
        {
            // state
            // 0 通常
            // 1 /一文字受付
            // 2 /* マルチラインコメント！
            // 3 /* *次はなんだよ……
            for (int raw = 0, state = 0, lineCount = *LineCountPtr; raw < lineCount; raw++)
            {
                var cptr = Contents + LineStarts[raw];
                int thisLineLength = LineLengths[raw];
                for (int i = 0; i < thisLineLength; ++i, ++cptr)
                {
                    switch (state)
                    {
                        case 0:
                            if (*cptr == '/')
                                state = 1;
                            break;
                        case 1:
                            switch (*cptr)
                            {
                                case '/':
                                    state = 0;
                                    LineLengths[raw] = i - 1;
                                    goto ENDOFLINE;
                                case '+':
                                    if (isDebug == 0)
                                        goto case '/';
                                    *(cptr - 1) = *cptr = ' ';
                                    state = 0;
                                    break;
                                case '*':
                                    state = 2;
                                    *(cptr - 1) = *cptr = ' ';
                                    break;
                                default:
                                    state = 0;
                                    break;
                            }
                            break;
                        case 2:
                            if (*cptr == '*')
                                state = 3;
                            *cptr = ' ';
                            break;
                        case 3:
                            if (*cptr == '/')
                                state = 0;
                            else state = 2;
                            *cptr = ' ';
                            break;
                    }
                }
            ENDOFLINE:
                for (int i = LineLengths[raw]; --i >= 0; --LineLengths[raw])
                {
                    ushort p = (Contents + LineStarts[raw])[i];
                    if (p != '\t' && p != ' ')
                        break;
                }
            }
            while (*LineCountPtr >= 0)
            {
                // 最終行が空行なら行数減らして？（はぁと）
                if (LineLengths[*LineCountPtr - 1] != 0)
                    return;
                *LineCountPtr -= 1;
            }
        }
    }
}