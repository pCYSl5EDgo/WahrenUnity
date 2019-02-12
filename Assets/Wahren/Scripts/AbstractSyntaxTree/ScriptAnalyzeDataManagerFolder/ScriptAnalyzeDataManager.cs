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
        private NativeArray<ParseJob> jobs;
        private NativeArray<ParseJob.CommonData> commonDatas;
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
            bool isNonePending = true;
            for (int i = jobs.Length; --i >= 0;)
            {
                ref var job = ref ((ParseJob*)jobs.GetUnsafePtr())[i];
                if (job.CommonPtr == null)
                {
                    continue;
                }
                var result = job.CommonPtr->Result;
                if (result.IsSuccess)
                {
                    job = default;
                }
                else if (result.IsError)
                {
                    job = default;
                }
                else if (result.IsPending)
                {
                    ReSchedule(ref job, job.CommonPtr);
                    isNonePending = false;
                }
            }
            if (isNonePending)
            {
                currentStage = Stage.Done;
            }
        }

        private void ReSchedule(ref ParseJob job, ParseJob.CommonData* common)
        {
            ref var result = ref common->Result;
            switch ((Location)(result.DataIndex >> 24))
            {
                case Location.Race:
                    LengthenRace(result);
                    break;
                default:
                    throw new System.NotImplementedException(this.FullPaths[result.Span.File] + "  " + ((Location)(result.DataIndex >> 24)).ToString() + " " + result.Span.ToString());
            }
            *Status = InterpreterStatus.None;
            result = new TryInterpretReturnValue(common->LastNameSpan, 0, InterpreterStatus.None);
            handles.Add(job.Schedule());
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
                    UnityEngine.Debug.Log("race ast value type pair lengthen");
                    ListUtility.Lengthen(ref ASTValueTypePairList.Values, ref ASTValueTypePairList.Capacity);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
                    UnityEngine.Debug.Log("race identifier number pair lengthen");
                    ListUtility.Lengthen(ref RaceParserTempData.Constis, ref RaceParserTempData.ConstiCapacity);
                    break;
                case PendingReason.SectionListCapacityShortage:
                    switch (result.SubDataIndex)
                    {
                        case 1: // name
                            UnityEngine.Debug.Log("race name lengthen");
                            ListUtility.Lengthen(ref RaceParserTempData.Names, ref RaceParserTempData.NameCapacity);
                            break;
                        case 2: // align
                            UnityEngine.Debug.Log("race align lengthen");
                            ListUtility.Lengthen(ref RaceParserTempData.Aligns, ref RaceParserTempData.AlignCapacity);
                            break;
                        case 3: // brave
                            UnityEngine.Debug.Log("race brave lengthen");
                            ListUtility.Lengthen(ref RaceParserTempData.Braves, ref RaceParserTempData.BraveCapacity);
                            break;
                        case 4: //consti
                            UnityEngine.Debug.Log("race consti lengthen");
                            ListUtility.Lengthen(ref RaceParserTempData.Constis, ref RaceParserTempData.ConstiCapacity);
                            break;
                        case 5: // movetype
                            UnityEngine.Debug.Log("race movetype lengthen");
                            ListUtility.Lengthen(ref RaceParserTempData.MoveTypes, ref RaceParserTempData.MoveTypeCapacity);
                            break;
                    }
                    break;
                case PendingReason.TreeListCapacityShortage:
                    UnityEngine.Debug.Log("race lengthen");
                    ListUtility.Lengthen(ref RaceParserTempData.Values, ref RaceParserTempData.Capacity);
                    break;
            }
        }

        private void CreateNewParseJob(TextFile file)
        {
            var cdata = (ParseJob.CommonData*)commonDatas.GetUnsafePtr() + file.FilePathId;
            cdata->LastStructKind = Location.None;
            cdata->Caret = new Caret { File = file.FilePathId, Line = 0, Column = 0 };
            cdata->LastNameSpan = new Span(cdata->Caret, 0);
            cdata->Result = new TryInterpretReturnValue(cdata->LastNameSpan, 0, InterpreterStatus.None);
            jobs[file.FilePathId] = new ParseJob
            {
                File = file,
                CancellationTokenPtr = Status,
                ScriptPtr = ScriptPtr,
                CommonPtr = cdata,
            };
        }
    }
}