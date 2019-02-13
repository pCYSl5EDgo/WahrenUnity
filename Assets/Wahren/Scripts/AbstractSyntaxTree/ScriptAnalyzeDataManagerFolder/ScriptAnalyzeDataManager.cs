﻿using Unity.Jobs;
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
                    ReSchedule(ref job);
                    isNonePending = false;
                }
            }
            if (isNonePending)
            {
                currentStage = Stage.Done;
            }
        }

        private void ReSchedule(ref ParseJob job)
        {
            ref var result = ref job.CommonPtr->Result;
            var (location, reason) = result;
            if (reason != PendingReason.Other)
            {
                switch (location)
                {
                    case Location.Race:
                        LengthenRace(result);
                        break;
                    case Location.MoveType:
                        LengthenMoveType(result);
                        break;
                    default:
                        throw new System.NotImplementedException(this.FullPaths[result.Span.File] + "  " + ((Location)(result.DataIndex >> 24)).ToString() + " " + result.Span.ToString());
                }
            }
            *Status = InterpreterStatus.None;
            result = new TryInterpretReturnValue(job.CommonPtr->LastNameSpan, 0, InterpreterStatus.None);
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

        private void LengthenMoveType(TryInterpretReturnValue result)
        {
            ref var tmpData = ref ScriptPtr->MoveTypeParserTempData;
            ref var identifierNumberPairs = ref tmpData.IdentifierNumberPairs;
            (_, var reason) = result;
            const string prefix = "movetype";
            switch (reason)
            {
                case PendingReason.ASTValueTypePairListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log(prefix + " ast value type pair lengthen\n" + result.ToString());
#endif
                    ListUtility.Lengthen(ref ASTValueTypePairList.Values, ref ASTValueTypePairList.Capacity);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log(prefix + " identifier number pair lengthen\n" + result.ToString() + "\n" + identifierNumberPairs.Capacity);
#endif
                    identifierNumberPairs.Lengthen();
                    break;
                case PendingReason.SectionListCapacityShortage:
                    switch (result.SubDataIndex)
                    {
                        case MoveTypeTree.name + 1: // name
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(prefix + " name lengthen\n" + result.ToString());
#endif
                            ListUtility.Lengthen(ref tmpData.Names, ref tmpData.NameCapacity);
                            break;
                        case MoveTypeTree.help + 1: // help
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(prefix + " help lengthen\n" + result.ToString());
#endif
                            ListUtility.Lengthen(ref tmpData.Helps, ref tmpData.HelpCapacity);
                            break;
                        case MoveTypeTree.consti + 1: // consti
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(prefix + " consti lengthen\n" + result.ToString());
#endif
                            ListUtility.Lengthen(ref tmpData.Constis, ref tmpData.ConstiCapacity);
                            break;
                    }
                    break;
                case PendingReason.TreeListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log(prefix + " lengthen\n" + result.ToString());
#endif
                    ListUtility.Lengthen(ref tmpData.Values, ref tmpData.Capacity);
                    break;
            }
        }

        private void LengthenRace(in TryInterpretReturnValue result)
        {
            ref var raceParserTempData = ref ScriptPtr->RaceParserTempData;
            ref var identifierNumberPairs = ref raceParserTempData.IdentifierNumberPairs;
            (_, var reason) = result;
            const string prefix = "race";
            switch (reason)
            {
                case PendingReason.ASTValueTypePairListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log(prefix + " ast value type pair lengthen\n" + result.ToString());
#endif
                    ListUtility.Lengthen(ref ASTValueTypePairList.Values, ref ASTValueTypePairList.Capacity);
                    break;
                case PendingReason.IdentifierNumberPairListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log(prefix + " identifier number pair lengthen\n" + result.ToString() + "\nCapacity: " + identifierNumberPairs.Capacity + " , Length: " + identifierNumberPairs.Length);
#endif
                    identifierNumberPairs.Lengthen();
                    break;
                case PendingReason.SectionListCapacityShortage:
                    switch (result.SubDataIndex)
                    {
                        case RaceTree.name + 1: // name
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(prefix + " name lengthen\n" + result.ToString());
#endif
                            ListUtility.Lengthen(ref raceParserTempData.Names, ref raceParserTempData.NameCapacity);
                            break;
                        case RaceTree.align + 1: // align
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(prefix + " align lengthen\n" + result.ToString());
#endif
                            ListUtility.Lengthen(ref raceParserTempData.Aligns, ref raceParserTempData.AlignCapacity);
                            break;
                        case RaceTree.brave + 1: // brave
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(prefix + " brave lengthen\n" + result.ToString());
#endif
                            ListUtility.Lengthen(ref raceParserTempData.Braves, ref raceParserTempData.BraveCapacity);
                            break;
                        case RaceTree.consti + 1: //consti
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(prefix + " consti lengthen\n" + result.ToString());
#endif
                            ListUtility.Lengthen(ref raceParserTempData.Constis, ref raceParserTempData.ConstiCapacity);
                            break;
                        case RaceTree.movetype + 1: // movetype
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(prefix + " movetype lengthen\n" + result.ToString());
#endif
                            ListUtility.Lengthen(ref raceParserTempData.MoveTypes, ref raceParserTempData.MoveTypeCapacity);
                            break;
                    }
                    break;
                case PendingReason.TreeListCapacityShortage:
#if UNITY_EDITOR
                    UnityEngine.Debug.Log(prefix + " lengthen\n" + result.ToString());
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