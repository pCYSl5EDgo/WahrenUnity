using System.IO;
using System.Text;

using UnityEngine;

using pcysl5edgo.Wahren;
using pcysl5edgo.Wahren.AST;

public class TestManager : MonoBehaviour
{
    [SerializeField] public string ScriptDirectoryFullPath;
    int frame;
    ScriptAnalyzeDataManager scriptManager;
    bool firstFrameDone = true;
    StringBuilder buffer;
    void Start()
    {
        frame = 0;
        buffer = new StringBuilder(1024);
    }

    void OnDestroy()
    {
        if (!(scriptManager is null))
        {
            scriptManager.Dispose();
            scriptManager = null;
        }
    }
    unsafe void Update()
    {
        if (scriptManager is null)
        {
            scriptManager = new ScriptAnalyzeDataManager(new DirectoryInfo(ScriptDirectoryFullPath).GetFiles("*.dat", SearchOption.AllDirectories), true, false);
            return;
        }
        switch (scriptManager.CurrentStage)
        {
            case ScriptAnalyzeDataManager.Stage.None:
                break;
            case ScriptAnalyzeDataManager.Stage.PreLoading:
                UnityEngine.Debug.Log("Frame : " + frame++ + "\nNow PreLoading");
                scriptManager.StartLoad();
                break;
            case ScriptAnalyzeDataManager.Stage.Loading:
                UnityEngine.Debug.Log("Frame : " + frame++ + "\nNow Loading");
                scriptManager.Update();
                break;
            case ScriptAnalyzeDataManager.Stage.PreParsing:
                UnityEngine.Debug.Log("Frame : " + frame++ + "\nNow PreParsing");
                scriptManager.StartParse();
                break;
            case ScriptAnalyzeDataManager.Stage.Parsing:
                UnityEngine.Debug.Log("Frame : " + frame++ + "\nNow Parsing");
                scriptManager.Update();
                break;
            case ScriptAnalyzeDataManager.Stage.GarbageCollecting:
                UnityEngine.Debug.Log("Frame : " + frame++ + "\nNow Garbage Collecting");
                scriptManager.Update();
                break;
            case ScriptAnalyzeDataManager.Stage.Done:
                if (firstFrameDone)
                {
                    UnityEngine.Debug.Log("Frame : " + frame++ + "\nDone!");
                    firstFrameDone = false;
                    for (int i = 0; i < scriptManager.RaceParserTempData.Length; i++)
                    {
                        if (i != 0)
                            buffer.AppendLine();
                        buffer.Append(scriptManager.RaceParserTempData.Values[i], scriptManager.Files, scriptManager.RaceParserTempData, scriptManager.IdentifierNumberPairList, scriptManager.ASTValueTypePairList);
                    }
                    UnityEngine.Debug.Log(buffer.ToString());
                }
                break;
        }
    }
}