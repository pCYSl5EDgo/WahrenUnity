using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    internal unsafe struct ScriptAnalyzeDataManager_Internal// : System.IDisposable
    {
        public TextFile* Files;
        public int FileLength;
        public RaceParserTempData RaceParserTempData;
        public MoveTypeParserTempData MoveTypeParserTempData;
        public ASTValueTypePairList ASTValueTypePairList;

        public static ScriptAnalyzeDataManager_Internal* CreatePtr(int length)
        {
            var answer = (ScriptAnalyzeDataManager_Internal*)UnsafeUtility.Malloc(sizeof(ScriptAnalyzeDataManager_Internal), 4, Allocator.Persistent);
            answer[0] = new ScriptAnalyzeDataManager_Internal(length);
            return answer;
        }
        private ScriptAnalyzeDataManager_Internal(int length)
        {
            FileLength = length;
            long size = sizeof(TextFile) * FileLength;
            Files = (TextFile*)UnsafeUtility.Malloc(size, 4, Allocator.Persistent);
            UnsafeUtility.MemClear(Files, size);
            RaceParserTempData = new RaceParserTempData(16);
            MoveTypeParserTempData = new MoveTypeParserTempData(16);
            ASTValueTypePairList = new ASTValueTypePairList(1024);
        }

        public void Dispose()
        {
            if (FileLength != 0)
            {
                for (int i = 0; i < FileLength; i++)
                {
                    if (Files + i != null)
                    {
                        Files[i].Dispose();
                    }
                }
                if (Files != null)
                {
                    UnsafeUtility.Free(Files, Allocator.Persistent);
                    Files = null;
                }
            }
            RaceParserTempData.Dispose();
            MoveTypeParserTempData.Dispose();
            ASTValueTypePairList.Dispose();
            this = default;
        }
    }
}