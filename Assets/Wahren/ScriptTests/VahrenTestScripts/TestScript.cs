using System.Text;
using NUnit.Framework;
using UnityEngine.TestTools;
using pcysl5edgo.Wahren.AST;

using A = UnityEngine.Assertions.Assert;
using System.Collections;
using Unity.Collections;

public class TestScript
{
    [UnityTest]
    public IEnumerator Vahren_Normal_Race_Movetype()
    {
        string ScriptDirectoryFullPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Wahren/ScriptTests/VahrenTestScripts/Race_Movetype_Normals");
        const bool IsUtf16 = false;
        const bool IsDebug = false;
        var buffer = new StringBuilder(1024);
        using (var scriptManager = new ScriptAnalyzeDataManager(new System.IO.DirectoryInfo(ScriptDirectoryFullPath).GetFiles("*.dat", System.IO.SearchOption.AllDirectories), IsUtf16, IsDebug, Allocator.Persistent, false))
        {
            yield return null;
            foreach (var _ in PreparationLoop(scriptManager))
                yield return null;
            const int RaceCount_Vahren = 12;
            int length = scriptManager.RaceParserTempData.Values.Length;
            A.AreEqual(RaceCount_Vahren, length);
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                    buffer.AppendLine();
                AppendRace(buffer, scriptManager, i);
            }
#if SHOW_LOG
            UnityEngine.Debug.Log(buffer.ToString());
#endif
            buffer.Clear();
            const int MovetypeCount_Vahren = 12;
            A.AreEqual(MovetypeCount_Vahren, scriptManager.MovetypeParserTempData.Values.Length);
            for (int i = 0, end = scriptManager.MovetypeParserTempData.Values.Length; i < end; i++)
            {
                if (i != 0)
                    buffer.AppendLine();
                AppendMovetype(buffer, scriptManager, i);
            }
#if SHOW_LOG
            UnityEngine.Debug.Log(buffer.ToString());
#endif
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
        using (var scriptManager = new ScriptAnalyzeDataManager(new System.IO.DirectoryInfo(ScriptDirectoryFullPath).GetFiles("*.dat", System.IO.SearchOption.AllDirectories), IsUtf16, IsDebug, Allocator.Persistent, false))
        {
            yield return null;
            foreach (var _ in PreparationLoop(scriptManager))
            {
                yield return null;
            }
        }
    }

    private static unsafe void AppendMovetype(StringBuilder buffer, ScriptAnalyzeDataManager scriptManager, int i) => buffer.AppendEx(scriptManager.MovetypeParserTempData.Values.GetRef<MovetypeTree>(i), scriptManager.Files, scriptManager.MovetypeParserTempData);

    private static unsafe void AppendRace(StringBuilder buffer, ScriptAnalyzeDataManager scriptManager, int i) => buffer.AppendEx(scriptManager.RaceParserTempData.Values.GetRef<RaceTree>(i), scriptManager.Files, scriptManager.RaceParserTempData);
}