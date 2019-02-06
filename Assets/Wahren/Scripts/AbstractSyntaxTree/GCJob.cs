using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    internal unsafe struct GCJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] public ScriptAnalyzeDataManager_Internal* ScriptPtr;
        [NativeDisableUnsafePtrRestriction] public RaceParserTempData.OldLengths* OldPtr;
        public int OldASTValueTypePairListLength;
        public int OldIdentifierNumberPairListLength;

        public void Execute()
        {
            var Old = *OldPtr;
            ref var raceParserTempData = ref ScriptPtr->RaceParserTempData;
            ref var astPairList = ref ScriptPtr->ASTValueTypePairList;
            ref var identifierNumberPairList = ref ScriptPtr->IdentifierNumberPairList;
            GC_ASTPairList(Old, ref raceParserTempData, ref astPairList);
            GC_Race_Consti(Old, ref raceParserTempData);
            /*
                残りはまだ実装しないでおく
             */
            return;
        }

        private static void GC_Race_Consti(RaceParserTempData.OldLengths Old, ref RaceParserTempData raceParserTempData)
        {
            for (int descendingIndex = raceParserTempData.ConstiLength; --descendingIndex >= Old.ConstiLength && descendingIndex != 0;)
            {
                ref var prev = ref raceParserTempData.Constis[descendingIndex - 1];
                ref var current = ref raceParserTempData.Constis[descendingIndex];
                var gap = current.Start - (prev.Start + prev.Length);
                if (gap == 0)
                    continue;
                ListUtility.MemMove(ref raceParserTempData.Constis, ref raceParserTempData.ConstiLength, prev.Start + prev.Length, current.Start);
                for (int ascendingIndex = descendingIndex; ascendingIndex < raceParserTempData.Length; ascendingIndex++)
                {
                    raceParserTempData.Constis[ascendingIndex].Start -= gap;
                }
            }
        }

        private static void GC_ASTPairList(RaceParserTempData.OldLengths Old, ref RaceParserTempData raceParserTempData, ref ASTValueTypePairList astPairList)
        {
            for (int descendingIndex = raceParserTempData.Length; --descendingIndex >= Old.Length && descendingIndex > 0;)
            {
                ref var prev = ref raceParserTempData.Values[descendingIndex - 1];
                ref var current = ref raceParserTempData.Values[descendingIndex];
                var gap = current.Start - (prev.Start + prev.Length);
                if (gap == 0)
                    continue;
                ListUtility.MemMove(ref astPairList.Values, ref astPairList.Length, prev.Start + prev.Length, current.Start);
                for (int ascendingIndex = descendingIndex; ascendingIndex < raceParserTempData.Length; ascendingIndex++)
                {
                    raceParserTempData.Values[ascendingIndex].Start -= gap;
                }
            }
        }
    }
}