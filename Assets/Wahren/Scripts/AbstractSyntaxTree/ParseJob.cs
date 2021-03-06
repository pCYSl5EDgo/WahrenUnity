﻿using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct ParseJob : IJob
    {
        internal ParseJob(InterpreterStatus* cancellationTokenPtr, ScriptAnalyzeDataManager_Internal* scriptPtr, TextFile file, CommonData* commonDataPtr, Allocator allocator)
        {
            this.CancellationTokenPtr = cancellationTokenPtr;
            this.ScriptPtr = scriptPtr;
            this.File = file;
            this.CommonPtr = commonDataPtr;
            this.allocator = allocator;
        }
        public struct CommonData
        {
            public Caret Caret;
            public Location LastStructKind;
            public Span LastNameSpan, LastParentNameSpan;
            public TryInterpretReturnValue Result;
        }
        [NativeDisableUnsafePtrRestriction] private InterpreterStatus* CancellationTokenPtr;
        [NativeDisableUnsafePtrRestriction] private ScriptAnalyzeDataManager_Internal* ScriptPtr;
        [NativeDisableUnsafePtrRestriction] private TextFile File;
        [NativeDisableUnsafePtrRestriction] public CommonData* CommonPtr;
        public Allocator allocator;
        public void Execute()
        {
            File.SkipWhiteSpace(ref CommonPtr->Caret);
            var startCaret = CommonPtr->Caret;
            var currentCaret = CommonPtr->Caret;
            ref var result = ref CommonPtr->Result;
            while (CommonPtr->Caret.Line < File.LineCount)
            {
                // 少なくとも構造体の種類を確定させ、その名前と親は詳らかにせよ
                // そして{までを読み終えた状態になるまでは他スレがメモリ不足で爆死しようとも作業するべし。
                File.SkipWhiteSpace(ref CommonPtr->Caret);
                if (CommonPtr->LastStructKind == Location.None)
                {
                    result = StructAnalyzer.TryGetFirstStructLocation(File.CurrentCharPointer(CommonPtr->Caret), File.LineLengths[CommonPtr->Caret.Line], CommonPtr->Caret, CommonPtr->Caret.Column);
                    if (result.IsError)
                    {
                        return;
                    }
                    CommonPtr->LastStructKind = (Location)result.SubDataIndex;
                    currentCaret = result.Span.CaretNextToEndOfThisSpan;
                    File.SkipWhiteSpace(ref currentCaret);
                    if (StructAnalyzer.IsStructKindWithName(CommonPtr->LastStructKind))
                    {
                        if (!(result = File.TryGetStructName(currentCaret)))
                        {
                            return;
                        }
                        CommonPtr->LastNameSpan = result.Span;
                        currentCaret = CommonPtr->LastNameSpan.CaretNextToEndOfThisSpan;
                        File.SkipWhiteSpace(ref currentCaret);
                        if (!(result = StructWithNameAnalyzer.TryGetParentStructName(File, currentCaret)))
                        {
                            return;
                        }
                        CommonPtr->LastParentNameSpan = result.Span;
                        currentCaret = CommonPtr->LastParentNameSpan.CaretNextToEndOfThisSpan;
                        File.SkipWhiteSpace(ref currentCaret);
                    }
                    if (!(result = currentCaret.IsCurrentCharEquals((File.Contents + File.LineStarts[currentCaret.Line])[currentCaret.Column], '{')))
                    {
                        return;
                    }
                    currentCaret = result.Span.CaretNextToEndOfThisSpan;
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
                        if (result = File.TryParseRaceStructMultiThread(ref ScriptPtr->RaceParserTempData, ref ScriptPtr->ASTValueTypePairList, CommonPtr->LastNameSpan, CommonPtr->LastParentNameSpan, currentCaret, out currentCaret, allocator))
                            goto SAVESUCCESS;
                        return;
                    case Location.Movetype:
                        if (result = File.TryParseMovetypeStructMultiThread(ref ScriptPtr->MovetypeParserTempData, ref ScriptPtr->ASTValueTypePairList, CommonPtr->LastNameSpan, CommonPtr->LastParentNameSpan, currentCaret, out currentCaret, allocator))
                            goto SAVESUCCESS;
                        return;
                    case Location.Voice:
                        if (result = File.TryParseVoiceStructMultiThread(ref ScriptPtr->VoiceParserTempData, ref ScriptPtr->ASTValueTypePairList, CommonPtr->LastNameSpan, CommonPtr->LastParentNameSpan, currentCaret, out currentCaret, allocator))
                            goto SAVESUCCESS;
                        return;
                    default:
                        return;
                }
            SAVESUCCESS:
                SaveSuccess(ref currentCaret);
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