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

        private NativeList<JobHandle> handles;
        private NativeArray<ParseJob> jobs;
        private NativeArray<ParseJob.CommonData> commonDatas;
        private Stage currentStage;
#if UNITY_EDITOR
        public bool ShowLog;
#endif

        private void ScheduleParsing()
        {
            for (int i = 0; i < jobs.Length; i++)
            {
                handles.Add(jobs[i].Schedule());
            }
            currentStage = Stage.Parsing;
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
            }
            if (isNonePending)
            {
                currentStage = Stage.Done;
            }
        }

        private void CreateNewParseJob(TextFile file)
        {
            var cdata = (ParseJob.CommonData*)commonDatas.GetUnsafePtr() + file.FilePathId;
            cdata->LastStructKind = Location.None;
            cdata->Caret = new Caret { File = file.FilePathId, Line = 0, Column = 0 };
            cdata->LastNameSpan = new Span(cdata->Caret, 0);
            cdata->Result = new TryInterpretReturnValue(cdata->LastNameSpan, 0, InterpreterStatus.None);
            jobs[file.FilePathId] = new ParseJob(Status, ScriptPtr, file, cdata, allocator);
        }
    }
}