using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    internal unsafe struct ScriptAnalyzeDataManager_Internal// : System.IDisposable
    {
        public TextFile* Files;
        public int FileLength;
        public RaceParserTempData RaceParserTempData;
        public MovetypeParserTempData MovetypeParserTempData;
        public ASTTypePageIndexPairListLinkedList ASTValueTypePairList;

        public static ScriptAnalyzeDataManager_Internal* CreatePtr(int length, Allocator allocator)
        {
            var answer = (ScriptAnalyzeDataManager_Internal*)UnsafeUtility.Malloc(sizeof(ScriptAnalyzeDataManager_Internal), 4, allocator);
            answer[0] = new ScriptAnalyzeDataManager_Internal(length, allocator);
            return answer;
        }
        private ScriptAnalyzeDataManager_Internal(int length, Allocator allocator)
        {
            FileLength = length;
            long size = sizeof(TextFile) * FileLength;
            Files = (TextFile*)UnsafeUtility.Malloc(size, 4, allocator);
            UnsafeUtility.MemClear(Files, size);
            RaceParserTempData = new RaceParserTempData(16, allocator);
            MovetypeParserTempData = new MovetypeParserTempData(16, allocator);
            ASTValueTypePairList = new ASTTypePageIndexPairListLinkedList(1024, allocator);
        }

        public void Dispose(Allocator allocator)
        {
            if (FileLength != 0)
            {
                for (int i = 0; i < FileLength; i++)
                {
                    if (Files + i != null)
                    {
                        Files[i].Dispose(allocator);
                    }
                }
                if (Files != null)
                {
                    UnsafeUtility.Free(Files, allocator);
                    Files = null;
                }
            }
            RaceParserTempData.Dispose(allocator);
            MovetypeParserTempData.Dispose(allocator);
            ASTValueTypePairList.Dispose(allocator);
            this = default;
        }
    }
}