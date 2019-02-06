using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe sealed class ScriptAnalyzeDataManager : System.IDisposable
    {
        private System.Text.StringBuilder buffer = new System.Text.StringBuilder(4096);
        internal InterpreterStatus* Status;
        internal ScriptAnalyzeDataManager_Internal* ScriptPtr;

        private RaceParserTempData.OldLengths* RaceOldLengths;
        private int OldASTValueTypePairListLength;
        private int OldIdentifierNumberPairListLength;

        public string[] FullPaths, Names;
        private NativeList<JobHandle> handles;
        private NativeList<ParseJob> jobs;
        internal int currentStage;

        public int CurrentStage => currentStage;
        public int CurrentJobLength => jobs.Length;

        public bool IsParsing => currentStage == 1;

        public void ParseStart()
        {
            for (int i = 0; i < ScriptPtr->FileLength; i++)
            {
                CreateNewParseJob(ScriptPtr->Files[i]);
            }
            Schedule();
        }

        private void Schedule()
        {
            for (int i = 0; i < jobs.Length; i++)
            {
                handles.Add(jobs[i].Schedule());
            }
            currentStage = 1;
            UnsafeUtility.MemCpy(RaceOldLengths, &ScriptPtr->RaceParserTempData, sizeof(RaceParserTempData.OldLengths));
            OldASTValueTypePairListLength = ScriptPtr->ASTValueTypePairList.Length;
            OldIdentifierNumberPairListLength = ScriptPtr->IdentifierNumberPairList.Length;
            *Status = InterpreterStatus.None;
        }

        public void Update()
        {
            switch (currentStage)
            {
                case 0:
                    return;
                case 1:
                    Case1();
                    break;
                case 2:
                    Case2();
                    break;
                case 3:
                    break;
            }
        }

        private void Case2()
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
            Schedule();
        }

        private void Case1()
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
            currentStage = 3;
        }

        private void GarbageCollection()
        {
            currentStage = 2;
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

        public static ScriptAnalyzeDataManager Create(ref ScriptLoadReturnValue disposableScript)
        {
            if (!disposableScript.IsCreated) throw new System.ArgumentNullException();
            var answer = new ScriptAnalyzeDataManager(disposableScript.FullPaths, disposableScript.Names);
            *answer.ScriptPtr = ScriptAnalyzeDataManager_Internal.Copy((TextFile*)disposableScript.Files.GetUnsafePtr(), disposableScript.Files.Length);
            disposableScript.Files.Dispose();
            disposableScript = default;
            return answer;
        }
        private ScriptAnalyzeDataManager(string[] fullPaths, string[] names)
        {
            Names = names;
            FullPaths = fullPaths;
            ScriptPtr = (ScriptAnalyzeDataManager_Internal*)UnsafeUtility.Malloc(sizeof(ScriptAnalyzeDataManager_Internal), 4, Allocator.Persistent);
            Status = (InterpreterStatus*)UnsafeUtility.Malloc(sizeof(InterpreterStatus), 4, Allocator.Persistent);
            handles = new NativeList<JobHandle>(Allocator.Persistent);
            jobs = new NativeList<ParseJob>(Allocator.Persistent);
            RaceOldLengths = (RaceParserTempData.OldLengths*)UnsafeUtility.Malloc(sizeof(RaceParserTempData.OldLengths), 4, Allocator.Persistent);
        }
        public void Dispose()
        {
            if (ScriptPtr != null)
            {
                ScriptPtr->Dispose();
                UnsafeUtility.Free(ScriptPtr, Allocator.Persistent);
                ScriptPtr = null;
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

    internal unsafe struct ScriptAnalyzeDataManager_Internal : System.IDisposable
    {
        internal TextFile* Files;
        internal int FileLength;
        internal RaceParserTempData RaceParserTempData;
        internal IdentifierNumberPairList IdentifierNumberPairList;
        internal ASTValueTypePairList ASTValueTypePairList;

        public ScriptAnalyzeDataManager_Internal(TextFile* files, int fileLength)
        {
            Files = files;
            FileLength = fileLength;
            RaceParserTempData = new RaceParserTempData(16);
            IdentifierNumberPairList = new IdentifierNumberPairList(256);
            ASTValueTypePairList = new ASTValueTypePairList(1024);
        }

        public static ScriptAnalyzeDataManager_Internal Copy(TextFile* files, int fileLength)
        {
            var answer = new ScriptAnalyzeDataManager_Internal
            {
                FileLength = fileLength,
                RaceParserTempData = new RaceParserTempData(16),
                IdentifierNumberPairList = new IdentifierNumberPairList(256),
                ASTValueTypePairList = new ASTValueTypePairList(1024),
            };
            answer.Files = (TextFile*)UnsafeUtility.Malloc(sizeof(TextFile) * fileLength, 4, Allocator.Persistent);
            UnsafeUtility.MemCpy(answer.Files, files, sizeof(TextFile) * fileLength);
            return answer;
        }
        public void Dispose()
        {
            if (FileLength != 0)
            {
                for (int i = 0; i < FileLength; i++)
                {
                    Files[i].Dispose();
                }
                UnsafeUtility.Free(Files, Allocator.Persistent);
            }
            RaceParserTempData.Dispose();
            IdentifierNumberPairList.Dispose();
            ASTValueTypePairList.Dispose();
            this = default;
        }
    }
}