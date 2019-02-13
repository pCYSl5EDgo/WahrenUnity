using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

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
            // 発行されているJob全てを調べ全てが完了しているならば次の処理へ、終わっていなければ次のフレームに投げる
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
            // ParseJobを読み解き、Pending状態があるならそれを解消して再びJobを発行する
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
                OldASTValueTypePairListLength = ScriptPtr->ASTValueTypePairList.Length,
                OldPtr = RaceOldLengths,
                ScriptPtr = ScriptPtr,
            }.Schedule());
        }

        private void LengthenRace(in TryInterpretReturnValue result)
        {
            ref var raceParserTempData = ref ScriptPtr->RaceParserTempData;
            ref var identifierNumberPairs = ref raceParserTempData.IdentifierNumberPairs;
            switch ((PendingReason)(result.DataIndex & 0xff))
            {
                case PendingReason.ASTValueTypePairListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log("race ast value type pair lengthen");
#endif
                    ListUtility.Lengthen(ref ASTValueTypePairList.Values, ref ASTValueTypePairList.Capacity);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log("race identifier number pair lengthen");
#endif
                    identifierNumberPairs.Lengthen();
                    break;
                case PendingReason.SectionListCapacityShortage:
                    switch (result.SubDataIndex)
                    {
                        case 1: // name
#if UNITY_EDITOR
                            UnityEngine.Debug.Log("race name lengthen");
#endif
                            ListUtility.Lengthen(ref raceParserTempData.Names, ref raceParserTempData.NameCapacity);
                            break;
                        case 2: // align
#if UNITY_EDITOR
                            UnityEngine.Debug.Log("race align lengthen");
#endif
                            ListUtility.Lengthen(ref raceParserTempData.Aligns, ref raceParserTempData.AlignCapacity);
                            break;
                        case 3: // brave
#if UNITY_EDITOR
                            UnityEngine.Debug.Log("race brave lengthen");
#endif
                            ListUtility.Lengthen(ref raceParserTempData.Braves, ref raceParserTempData.BraveCapacity);
                            break;
                        case 4: //consti
#if UNITY_EDITOR
                            UnityEngine.Debug.Log("race consti lengthen");
#endif
                            ListUtility.Lengthen(ref raceParserTempData.Constis, ref raceParserTempData.ConstiCapacity);
                            break;
                        case 5: // movetype
#if UNITY_EDITOR
                            UnityEngine.Debug.Log("race movetype lengthen");
#endif
                            ListUtility.Lengthen(ref raceParserTempData.MoveTypes, ref raceParserTempData.MoveTypeCapacity);
                            break;
                    }
                    break;
                case PendingReason.TreeListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log("race lengthen");
#endif
                    ListUtility.Lengthen(ref raceParserTempData.Values, ref raceParserTempData.Capacity);
                    break;
            }
            // if (ScriptPtr->ASTValueTypePairList.Capacity == ScriptPtr->ASTValueTypePairList.Length)
            // {
            //     ListUtility.Lengthen(ref ScriptPtr->ASTValueTypePairList.Values, ref ScriptPtr->ASTValueTypePairList.Capacity);
            // }
            // if (identifierNumberPairs.Length == identifierNumberPairs.Capacity)
            // {
            //     identifierNumberPairs.Lengthen();
            // }
            // if (raceParserTempData.NameCapacity == raceParserTempData.NameLength)
            // {
            //     ListUtility.Lengthen(ref raceParserTempData.Names, ref raceParserTempData.NameCapacity);
            // }
            // if (raceParserTempData.AlignCapacity == raceParserTempData.AlignLength)
            // {
            //     ListUtility.Lengthen(ref raceParserTempData.Aligns, ref raceParserTempData.AlignCapacity);
            // }
            // if (raceParserTempData.BraveCapacity == raceParserTempData.BraveLength)
            // {
            //     ListUtility.Lengthen(ref raceParserTempData.Braves, ref raceParserTempData.BraveCapacity);
            // }
            // if (raceParserTempData.ConstiCapacity == raceParserTempData.ConstiLength)
            // {
            //     ListUtility.Lengthen(ref raceParserTempData.Constis, ref raceParserTempData.ConstiCapacity);
            // }
            // if (raceParserTempData.MoveTypeCapacity == raceParserTempData.MoveTypeLength)
            // {
            //     ListUtility.Lengthen(ref raceParserTempData.MoveTypes, ref raceParserTempData.MoveTypeCapacity);
            // }
            // if (raceParserTempData.Capacity == raceParserTempData.Length)
            // {
            //     ListUtility.Lengthen(ref raceParserTempData.Values, ref raceParserTempData.Capacity);
            // }
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