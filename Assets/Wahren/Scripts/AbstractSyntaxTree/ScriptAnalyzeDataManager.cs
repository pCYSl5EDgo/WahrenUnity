using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe sealed class ScriptAnalyzeDataManager : System.IDisposable
    {
        internal ScriptAnalyzeDataManager_Internal* _ScriptAnalyzeDataManager;
        private readonly string[] FullPaths, Names;
        public static ScriptAnalyzeDataManager Create(ref ScriptLoadReturnValue disposableScript)
        {
            if (!disposableScript.IsCreated) throw new System.ArgumentNullException();
            var answer = new ScriptAnalyzeDataManager(disposableScript.FullPaths, disposableScript.Names);
            *answer._ScriptAnalyzeDataManager = ScriptAnalyzeDataManager_Internal.Copy((TextFile*)disposableScript.Files.GetUnsafePtr(), disposableScript.Files.Length);
            disposableScript.Files.Dispose();
            disposableScript = default;
            return answer;
        }
        private ScriptAnalyzeDataManager(string[] fullPaths, string[] names)
        {
            Names = names;
            FullPaths = fullPaths;
        }
        public void Dispose()
        {
            _ScriptAnalyzeDataManager->Dispose();
        }
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