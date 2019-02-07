using System;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    unsafe partial class ScriptAnalyzeDataManager
    {
        public enum Stage
        {
            None, Loading, Parsing, GarbageCollecting, Done,
        }

        public Stage CurrentStage => currentStage;

        public void Update()
        {
            switch (currentStage)
            {
                case Stage.None:
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
                case Stage.Done:
                    break;
            }
        }
        public ScriptAnalyzeDataManager(System.IO.FileInfo[] fileInfos, bool isUtf16, bool isDebug)
        {
            if (fileInfos is null) throw new ArgumentNullException();
            FullPaths = new string[fileInfos.Length];
            Names = new string[fileInfos.Length];
            for (int i = 0; i < FullPaths.Length; i++)
            {
                FullPaths[i] = fileInfos[i].FullName;
                Names[i] = fileInfos[i].Name;
            }
            ScriptPtr = (ScriptAnalyzeDataManager_Internal*)UnsafeUtility.Malloc(sizeof(ScriptAnalyzeDataManager_Internal), 4, Allocator.Persistent);
            *ScriptPtr = ScriptAnalyzeDataManager_Internal.Create(FullPaths.Length);
            Status = (InterpreterStatus*)UnsafeUtility.Malloc(sizeof(InterpreterStatus), 4, Allocator.Persistent);
            handles = new NativeList<JobHandle>(Allocator.Persistent);
            jobs = new NativeList<ParseJob>(Allocator.Persistent);
            RaceOldLengths = (RaceParserTempData.OldLengths*)UnsafeUtility.Malloc(sizeof(RaceParserTempData.OldLengths), 4, Allocator.Persistent);
            InitialReadTempDataPtr = (InitialReadTempData*)UnsafeUtility.Malloc(sizeof(InitialReadTempData), 4, Allocator.Persistent);
            *InitialReadTempDataPtr = InitialReadTempData.Create(fileInfos, ScriptPtr->FileLength, &ScriptPtr->Files, isUtf16, isDebug);
            currentStage = Stage.Loading;
        }
        public void Dispose()
        {
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
            if (handles.IsCreated)
                handles.Dispose();
            if (jobs.IsCreated)
                jobs.Dispose();
            if (Status != null)
                UnsafeUtility.Free(Status, Allocator.Persistent);
            if (RaceOldLengths != null)
                UnsafeUtility.Free(RaceOldLengths, Allocator.Persistent);
            FullPaths = null;
            Names = null;
        }

        public ref RaceParserTempData RaceParserTempData => ref ScriptPtr->RaceParserTempData;
        public ref TextFile this[int index] => ref ScriptPtr->Files[index];
        public ref int Length => ref ScriptPtr->FileLength;
        public ref TextFile* Files => ref ScriptPtr->Files;
        public ref IdentifierNumberPairList IdentifierNumberPairList => ref ScriptPtr->IdentifierNumberPairList;
        public ref ASTValueTypePairList ASTValueTypePairList => ref ScriptPtr->ASTValueTypePairList;

    }
}