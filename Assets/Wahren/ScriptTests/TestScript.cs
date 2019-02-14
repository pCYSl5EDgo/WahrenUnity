using System.Text;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using pcysl5edgo.Wahren.AST;

using A = UnityEngine.Assertions.Assert;
using System;
using System.Collections.Generic;

public class TestScript
{
    [UnityTest]
    public IEnumerator Vahren_Normal_Race_Movetype()
    {
        string ScriptDirectoryFullPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Wahren/ScriptTests/VahrenTestScripts/Race_Movetype_Normals");
        const bool IsUtf16 = false;
        const bool IsDebug = false;
        var buffer = new StringBuilder(1024);
        using (var scriptManager = new ScriptAnalyzeDataManager(new System.IO.DirectoryInfo(ScriptDirectoryFullPath).GetFiles("*.dat", System.IO.SearchOption.AllDirectories), IsUtf16, IsDebug, false))
        {
            yield return null;
            foreach (var _ in PreparationLoop(scriptManager))
                yield return null;
            const int RaceCount_Vahren = 12;
            A.AreEqual(RaceCount_Vahren, scriptManager.RaceParserTempData.Length);
            for (int i = 0; i < scriptManager.RaceParserTempData.Length; i++)
            {
                if (i != 0)
                    buffer.AppendLine();
                AppendRace(buffer, scriptManager, i);
            }
            UnityEngine.Debug.Log(buffer.ToString());
            buffer.Clear();
            const int MovetypeCount_Vahren = 12;
            A.AreEqual(MovetypeCount_Vahren, scriptManager.MovetypeParserTempData.Length);
            for (int i = 0; i < scriptManager.MovetypeParserTempData.Length; i++)
            {
                if (i != 0)
                    buffer.AppendLine();
                AppendMovetype(buffer, scriptManager, i);
            }
            UnityEngine.Debug.Log(buffer.ToString());
        }
    }

    private IEnumerable PreparationLoop(ScriptAnalyzeDataManager scriptManager)
    {
        while (scriptManager.CurrentStage != ScriptAnalyzeDataManager.Stage.Done)
        {
            switch (scriptManager.CurrentStage)
            {
                case ScriptAnalyzeDataManager.Stage.None:
                    Assert.Fail("Stage must not be 'None'");
                    yield return null;
                    break;
                case ScriptAnalyzeDataManager.Stage.PreLoading:
                    scriptManager.StartLoad();
                    yield return null;
                    break;
                case ScriptAnalyzeDataManager.Stage.Loading:
                    scriptManager.Update();
                    yield return null;
                    break;
                case ScriptAnalyzeDataManager.Stage.PreParsing:
                    scriptManager.StartParse();
                    yield return null;
                    break;
                case ScriptAnalyzeDataManager.Stage.Parsing:
                    scriptManager.Update();
                    yield return null;
                    break;
                case ScriptAnalyzeDataManager.Stage.GarbageCollecting:
                    scriptManager.Update();
                    yield return null;
                    break;
            }
        }
    }

    [UnityTest]
    public IEnumerator Vahren_Abnormal_Race_Movetype()
    {
        string ScriptDirectoryFullPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Wahren/ScriptTests/VahrenTestScripts/Race_Movetype_Abnormals");
        const bool IsUtf16 = false;
        const bool IsDebug = false;
        var buffer = new StringBuilder(1024);
        using (var scriptManager = new ScriptAnalyzeDataManager(new System.IO.DirectoryInfo(ScriptDirectoryFullPath).GetFiles("*.dat", System.IO.SearchOption.AllDirectories), IsUtf16, IsDebug, false))
        {
            yield return null;
            foreach (var _ in PreparationLoop(scriptManager))
            {
                yield return null;
            }
        }
    }

    private static unsafe void AppendMovetype(StringBuilder buffer, ScriptAnalyzeDataManager scriptManager, int i)
    {
        buffer.AppendEx(scriptManager.MovetypeParserTempData.Values[i], scriptManager.Files, scriptManager.MovetypeParserTempData, scriptManager.ASTValueTypePairList);
    }

    private static unsafe void AppendRace(StringBuilder buffer, ScriptAnalyzeDataManager scriptManager, int i)
    {
        buffer.AppendEx(scriptManager.RaceParserTempData.Values[i], scriptManager.Files, scriptManager.RaceParserTempData, scriptManager.ASTValueTypePairList);
    }
}