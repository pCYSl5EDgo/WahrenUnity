using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe class ScriptAnalyzeDataManager
    {
        ScriptAnalyzeDataManager_Internal* _ScriptAnalyzeDataManager;
    }

    internal unsafe struct ScriptAnalyzeDataManager_Internal
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
    }
}