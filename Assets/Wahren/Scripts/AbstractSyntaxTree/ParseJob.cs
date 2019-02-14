using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    [BurstCompile]
    public unsafe struct ParseJob : IJob
    {
        public struct CommonData
        {
            public Caret Caret;
            public Location LastStructKind;
            public Span LastNameSpan, LastParentNameSpan;
            public TryInterpretReturnValue Result;
        }
        [NativeDisableUnsafePtrRestriction] public InterpreterStatus* CancellationTokenPtr;
        [NativeDisableUnsafePtrRestriction] internal ScriptAnalyzeDataManager_Internal* ScriptPtr;
        [NativeDisableUnsafePtrRestriction] public TextFile File;
        [NativeDisableUnsafePtrRestriction] public CommonData* CommonPtr;
        public void Execute()
        {
            File.SkipWhiteSpace(ref CommonPtr->Caret);
            var startCaret = CommonPtr->Caret;
            var currentCaret = CommonPtr->Caret;
            while (CommonPtr->Caret.Line < File.LineCount)
            {
                // 少なくとも構造体の種類を確定させ、その名前と親は詳らかにせよ
                // そして{までを読み終えた状態になるまでは他スレがメモリ不足で爆死しようとも作業するべし。
                File.SkipWhiteSpace(ref CommonPtr->Caret);
                if (CommonPtr->LastStructKind == Location.None)
                {
                    CommonPtr->Result = StructAnalyzer.TryGetFirstStructLocation(File.CurrentCharPointer(CommonPtr->Caret), File.LineLengths[CommonPtr->Caret.Line], CommonPtr->Caret, CommonPtr->Caret.Column);
                    if (CommonPtr->Result.IsError)
                    {
                        return;
                    }
                    CommonPtr->LastStructKind = (Location)CommonPtr->Result.SubDataIndex;
                    currentCaret = CommonPtr->Result.Span.CaretNextToEndOfThisSpan;
                    File.SkipWhiteSpace(ref currentCaret);
                    if (StructAnalyzer.IsStructKindWithName(CommonPtr->LastStructKind))
                    {
                        if (!(CommonPtr->Result = File.TryGetStructName(currentCaret)))
                        {
                            return;
                        }
                        CommonPtr->LastNameSpan = CommonPtr->Result.Span;
                        currentCaret = CommonPtr->LastNameSpan.CaretNextToEndOfThisSpan;
                        File.SkipWhiteSpace(ref currentCaret);
                        if (!(CommonPtr->Result = StructWithNameAnalyzer.TryGetParentStructName(File, currentCaret)))
                        {
                            return;
                        }
                        CommonPtr->LastParentNameSpan = CommonPtr->Result.Span;
                        currentCaret = CommonPtr->LastParentNameSpan.CaretNextToEndOfThisSpan;
                        File.SkipWhiteSpace(ref currentCaret);
                    }
                    if (!(CommonPtr->Result = currentCaret.IsCurrentCharEquals((File.Contents + File.LineStarts[currentCaret.Line])[currentCaret.Column], '{')))
                    {
                        return;
                    }
                    currentCaret = CommonPtr->Result.Span.CaretNextToEndOfThisSpan;
                }
                File.SkipWhiteSpace(ref currentCaret);
                CommonPtr->Caret = currentCaret;
                // 他スレから停止命令が出ているなら中断して(はぁと)
                if (*CancellationTokenPtr != InterpreterStatus.None)
                {
                    CommonPtr->Caret = startCaret;
                    return;
                }
                switch (CommonPtr->LastStructKind)
                {
                    case Location.Race:
                        if (CommonPtr->Result = File.TryParseRaceStructMultiThread(ref ScriptPtr->RaceParserTempData, ref ScriptPtr->ASTValueTypePairList, CommonPtr->LastNameSpan, CommonPtr->LastParentNameSpan, currentCaret, out currentCaret, out _))
                        {
                            SaveSuccess(ref currentCaret);
                            continue;
                        }
                        if (CommonPtr->Result.IsPending)
                        {
                            *CancellationTokenPtr = InterpreterStatus.Pending;
                            return;
                        }
                        break;
                    case Location.MoveType:
                        CommonPtr->Result = File.TryParseMovetypeStructMultiThread(ref ScriptPtr->MoveTypeParserTempData, ref ScriptPtr->ASTValueTypePairList, CommonPtr->LastNameSpan, CommonPtr->LastParentNameSpan, currentCaret, out currentCaret, out _);
                        if (CommonPtr->Result)
                        {
                            SaveSuccess(ref currentCaret);
                            continue;
                        }
                        if (CommonPtr->Result.IsPending)
                        {
                            *CancellationTokenPtr = InterpreterStatus.Pending;
                            var (location, _) = CommonPtr->Result;
                            return;
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        private void SaveSuccess(ref Caret currentCaret)
        {
            File.SkipWhiteSpace(ref currentCaret);
            CommonPtr->Caret = currentCaret;
            CommonPtr->LastStructKind = Location.None;
            CommonPtr->LastNameSpan = CommonPtr->LastParentNameSpan = default;
        }
    }
}