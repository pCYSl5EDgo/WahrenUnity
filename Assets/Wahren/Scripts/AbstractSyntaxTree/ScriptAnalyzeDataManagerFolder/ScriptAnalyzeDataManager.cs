using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe sealed partial class ScriptAnalyzeDataManager// : System.IDisposable
    {
        private System.Text.StringBuilder buffer = new System.Text.StringBuilder(4096);
        private InitialReadTempData* InitialReadTempDataPtr;
        private InterpreterStatus* Status;
        private ScriptAnalyzeDataManager_Internal* ScriptPtr;

        private RaceParserTempData.OldLengths* RaceOldLengths;
        private int OldASTValueTypePairListLength;
        private int OldIdentifierNumberPairListLength;

        private NativeList<JobHandle> handles;
        private NativeList<ParseJob> jobs;
        private Stage currentStage;

        private void ScheduleParsing()
        {
            for (int i = 0; i < jobs.Length; i++)
            {
                handles.Add(jobs[i].Schedule());
            }
            currentStage = Stage.Parsing;
            UnsafeUtility.MemCpy(RaceOldLengths, &ScriptPtr->RaceParserTempData, sizeof(RaceParserTempData.OldLengths));
            OldASTValueTypePairListLength = ScriptPtr->ASTValueTypePairList.Length;
            OldIdentifierNumberPairListLength = ScriptPtr->IdentifierNumberPairList.Length;
            *Status = InterpreterStatus.None;
        }

        private void LoadingUpdate()
        {
            if (InitialReadTempDataPtr == null) return;
            switch (InitialReadTempDataPtr->CurrentStage)
            {
                case InitialReadTempData.Stage.Done:
                    InitialReadTempDataPtr->Dispose();
                    UnsafeUtility.Free(InitialReadTempDataPtr, Allocator.Persistent);
                    InitialReadTempDataPtr = null;
                    currentStage = Stage.PreParsing;
                    break;
                default:
                    InitialReadTempDataPtr->Update();
                    break;
            }
        }

        private void GCUpdate()
        {
            bool isAnyFail = false;
            for (int i = handles.Length; --i >= 0;)
            {
                var jobHandle = handles[i];
                if (jobHandle.IsCompleted)
                {
                    jobHandle.Complete();
                    handles.RemoveAtSwapBack(i);
                }
                else
                {
                    isAnyFail = true;
                }
            }
            if (isAnyFail) return;
            ScheduleParsing();
        }

        private void ParseUpdate()
        {
            bool isAnyFail = false;
            for (int i = handles.Length; --i >= 0;)
            {
                var jobHandle = handles[i];
                if (jobHandle.IsCompleted)
                {
                    jobHandle.Complete();
                    handles.RemoveAtSwapBack(i);
                }
                else
                {
                    isAnyFail = true;
                }
            }
            if (isAnyFail)
            {
                return;
            }
            for (int i = jobs.Length; --i >= 0;)
            {
                var job = jobs[i];
                ref var result = ref job.Result;
                if (result.IsSuccess)
                {
                    jobs.RemoveAtSwapBack(i);
                    continue;
                }
                else if (result.IsError)
                {
                    jobs.RemoveAtSwapBack(i);
                    continue;
                }
                if (result.IsPending)
                {
                    switch ((Location)(result.DataIndex >> 24))
                    {
                        case Location.Race:
                            LengthenRace(result);
                            break;
                        default:
                            throw new System.NotImplementedException();
                    }
                }
            }
            currentStage = Stage.Done;
        }

        private void GarbageCollection()
        {
            currentStage = Stage.GarbageCollecting;
            if (handles.Length != 0)
                handles.Clear();
            handles.Add(new GCJob
            {
                OldIdentifierNumberPairListLength = ScriptPtr->IdentifierNumberPairList.Length,
                OldASTValueTypePairListLength = ScriptPtr->ASTValueTypePairList.Length,
                OldPtr = RaceOldLengths,
                ScriptPtr = ScriptPtr,
            }.Schedule());
        }

        private void LengthenRace(in TryInterpretReturnValue result)
        {
            switch ((PendingReason)(result.DataIndex & 0xff))
            {
                case PendingReason.ASTValueTypePairListCapacityShortage:
                    ListUtility.Lengthen(ref ASTValueTypePairList.Values, ref ASTValueTypePairList.Capacity);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
                    ListUtility.Lengthen(ref RaceParserTempData.Constis, ref RaceParserTempData.ConstiCapacity);
                    break;
                case PendingReason.SectionListCapacityShortage:
                    switch (result.SubDataIndex)
                    {
                        case 1: // name
                            ListUtility.Lengthen(ref RaceParserTempData.Names, ref RaceParserTempData.NameCapacity);
                            break;
                        case 2: // align
                            ListUtility.Lengthen(ref RaceParserTempData.Aligns, ref RaceParserTempData.AlignCapacity);
                            break;
                        case 3: // brave
                            ListUtility.Lengthen(ref RaceParserTempData.Braves, ref RaceParserTempData.BraveCapacity);
                            break;
                        case 4: //consti
                            ListUtility.Lengthen(ref RaceParserTempData.Constis, ref RaceParserTempData.ConstiCapacity);
                            break;
                        case 5: // movetype
                            ListUtility.Lengthen(ref RaceParserTempData.MoveTypes, ref RaceParserTempData.MoveTypeCapacity);
                            break;
                    }
                    break;
                case PendingReason.TreeListCapacityShortage:
                    ListUtility.Lengthen(ref RaceParserTempData.Values, ref RaceParserTempData.Capacity);
                    break;
            }
        }

        private void CreateNewParseJob(TextFile file)
        {
            var job = new ParseJob
            {
                File = file,
                CancellationTokenPtr = Status,
                Caret = new Caret { File = file.FilePathId, Line = 0, Column = 0 },
                LastStructKind = Location.None,
                ScriptPtr = ScriptPtr,
            };
            job.LastNameSpan.Start = job.LastParentNameSpan.Start = job.Caret;
            job.LastNameSpan.Length = job.LastParentNameSpan.Length = 0;
            job.Result = new TryInterpretReturnValue(job.LastNameSpan, 0, InterpreterStatus.None);
            jobs.Add(job);
        }
    }
}