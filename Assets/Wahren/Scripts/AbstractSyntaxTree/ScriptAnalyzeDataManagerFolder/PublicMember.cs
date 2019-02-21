using System;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    unsafe partial class ScriptAnalyzeDataManager
    {
        public System.IO.FileInfo[] FileInfos;
        public string[] FullPaths, Names;

        public enum Stage
        {
            None, PreLoading, Loading, PreParsing, Parsing, GarbageCollecting, Done,
        }

        public Stage CurrentStage => currentStage;

        public ScriptAnalyzeDataManager(System.IO.FileInfo[] fileInfos, bool isUtf16, bool isDebug
#if UNITY_EDITOR
        , bool showLog
#endif
        )
        {
            if (fileInfos is null) throw new ArgumentNullException();
#if UNITY_EDITOR
            ShowLog = showLog;
#endif
            FileInfos = fileInfos;
            FullPaths = new string[FileInfos.Length];
            Names = new string[FileInfos.Length];
            for (int i = 0; i < FullPaths.Length; i++)
            {
                FullPaths[i] = FileInfos[i].FullName;
                Names[i] = FileInfos[i].Name;
            }
            ScriptPtr = ScriptAnalyzeDataManager_Internal.CreatePtr(FullPaths.Length);
            Status = (InterpreterStatus*)UnsafeUtility.Malloc(sizeof(InterpreterStatus), 4, Allocator.Persistent);
            handles = new NativeList<JobHandle>(FullPaths.Length, Allocator.Persistent);
            jobs = new NativeArray<ParseJob>(FullPaths.Length, Allocator.Persistent);
            commonDatas = new NativeArray<ParseJob.CommonData>(FullPaths.Length, Allocator.Persistent);
            RaceOldLengths = (RaceParserTempData.OldLengths*)UnsafeUtility.Malloc(sizeof(RaceParserTempData.OldLengths), 4, Allocator.Persistent);
            InitialReadTempDataPtr = InitialReadTempData.CreatePtr(FileInfos, ScriptPtr->FileLength, &ScriptPtr->Files, isUtf16, isDebug);
            currentStage = Stage.PreLoading;
        }

        public void StartLoad()
        {
            if (currentStage != Stage.PreLoading)
            {
                throw new InvalidOperationException();
            }
            InitialReadTempDataPtr->StartLoad(FileInfos, FullPaths);
            currentStage = Stage.Loading;
        }

        public void StartParse()
        {
            if (currentStage != Stage.PreParsing)
            {
                throw new InvalidOperationException();
            }
            for (int i = 0; i < ScriptPtr->FileLength; i++)
            {
                CreateNewParseJob(ScriptPtr->Files[i]);
            }
            ScheduleParsing();
        }

        public void Update()
        {
            switch (currentStage)
            {
                case Stage.None:
                case Stage.PreLoading:
                case Stage.PreParsing:
                case Stage.Done:
                    return;
                case Stage.Loading:
                    LoadingUpdate();
                    return;
                case Stage.Parsing:
                    ParseUpdate();
                    break;
                case Stage.GarbageCollecting:
                    GCUpdate();
                    break;
            }
        }

        public void Dispose()
        {
            if (handles.IsCreated)
                handles.Dispose();
            if (jobs.IsCreated)
                jobs.Dispose();
            if (commonDatas.IsCreated)
                commonDatas.Dispose();
            if (Status != null)
                UnsafeUtility.Free(Status, Allocator.Persistent);
            if (RaceOldLengths != null)
                UnsafeUtility.Free(RaceOldLengths, Allocator.Persistent);
            FullPaths = null;
            Names = null;
            if (ScriptPtr != null)
            {
                ScriptPtr->Dispose();
                UnsafeUtility.Free(ScriptPtr, Allocator.Persistent);
                ScriptPtr = null;
            }
            if (InitialReadTempDataPtr != null)
            {
                InitialReadTempDataPtr->Dispose();
                UnsafeUtility.Free(InitialReadTempDataPtr, Allocator.Persistent);
                InitialReadTempDataPtr = null;
            }
        }

        public ref RaceParserTempData RaceParserTempData => ref ScriptPtr->RaceParserTempData;
        public ref MovetypeParserTempData MovetypeParserTempData => ref ScriptPtr->MovetypeParserTempData;
        public ref TextFile this[int index] => ref ScriptPtr->Files[index];
        public ref int Length => ref ScriptPtr->FileLength;
        public ref TextFile* Files => ref ScriptPtr->Files;
        public ref ASTValueTypePairList ASTValueTypePairList => ref ScriptPtr->ASTValueTypePairList;
    }
}