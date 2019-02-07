using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    internal unsafe struct ScriptAnalyzeDataManager_Internal : System.IDisposable
    {
        public TextFile* Files;
        public int FileLength;
        public RaceParserTempData RaceParserTempData;
        public IdentifierNumberPairList IdentifierNumberPairList;
        public ASTValueTypePairList ASTValueTypePairList;

        public static ScriptAnalyzeDataManager_Internal Create(int fileLength)
        => new ScriptAnalyzeDataManager_Internal
        {
            FileLength = fileLength,
            RaceParserTempData = new RaceParserTempData(16),
            IdentifierNumberPairList = new IdentifierNumberPairList(256),
            ASTValueTypePairList = new ASTValueTypePairList(1024),
        };

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