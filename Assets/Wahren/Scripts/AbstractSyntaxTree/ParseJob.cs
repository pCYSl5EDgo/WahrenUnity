using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    // [BurstCompile]
    public unsafe struct ParseJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] public InterpreterStatus* CancellationTokenPtr;
        [NativeDisableUnsafePtrRestriction] internal ScriptAnalyzeDataManager_Internal* ScriptPtr;
        [NativeDisableUnsafePtrRestriction] public TextFile File;
        public Caret Caret;
        public Location LastStructKind;
        public Span LastNameSpan, LastParentNameSpan;
        public TryInterpretReturnValue Result;
        public void Execute()
        {
            File.SkipWhiteSpace(ref Caret);
            var startCaret = Caret;
            while (Caret.Line < File.LineCount)
            {
                // 少なくとも構造体の種類を確定させ、その名前と親は詳らかにせよ
                // そして{までを読み終えた状態になるまでは他スレがメモリ不足で爆死しようとも作業するべし。
                Result = StructAnalyzer.TryGetFirstStructLocation(ref File, Caret);
                if (Result.IsError)
                {
                    return;
                }
                LastStructKind = (Location)Result.SubDataIndex;
                var currentCaret = Result.Span.CaretNextToEndOfThisSpan;
                File.SkipWhiteSpace(ref currentCaret);
                if (StructAnalyzer.IsStructKindWithName(LastStructKind))
                {
                    if (!(Result = File.TryGetStructName(currentCaret)))
                    {
                        return;
                    }
                    LastNameSpan = Result.Span;
                    currentCaret = LastNameSpan.CaretNextToEndOfThisSpan;
                    File.SkipWhiteSpace(ref currentCaret);
                    if (!(Result = StructWithNameAnalyzer.TryGetParentStructName(ref File, currentCaret)))
                    {
                        return;
                    }
                    LastParentNameSpan = Result.Span;
                    currentCaret = LastParentNameSpan.CaretNextToEndOfThisSpan;
                    File.SkipWhiteSpace(ref currentCaret);
                }
                if (!(Result = File.IsCurrentCharEquals(currentCaret, '{')))
                {
                    return;
                }
                currentCaret = Result.Span.CaretNextToEndOfThisSpan;
                File.SkipWhiteSpace(ref currentCaret);
                Caret = currentCaret;
                // 他スレから停止命令が出ているなら中断して(はぁと)
                if (*CancellationTokenPtr != InterpreterStatus.None)
                {
                    Caret = startCaret;
                    return;
                }
                switch (LastStructKind)
                {
                    case Location.Race:
                        if (Result = File.TryParseRaceStructMultiThread(ref ScriptPtr->RaceParserTempData, ref ScriptPtr->IdentifierNumberPairList, ref ScriptPtr->ASTValueTypePairList, LastNameSpan, LastParentNameSpan, currentCaret, out currentCaret, out _))
                        {
                            File.SkipWhiteSpace(ref currentCaret);
                            Caret = currentCaret;
                            continue;
                        }
                        if (Result.IsPending)
                        {
                            *CancellationTokenPtr = InterpreterStatus.Pending;
                        }
                        return;
                    default:
                        return;
                }
            }
        }
    }
}