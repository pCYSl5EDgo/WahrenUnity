using System;
using System.Text;

using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using pcysl5edgo.Wahren;
using pcysl5edgo.Wahren.AST;

public class TestManager : MonoBehaviour
{
    RawScriptLoadReturnValue rawScriptLoadReturnValue;
    ScriptLoadReturnValue scriptLoadReturnValue;
    [SerializeField] public string ScriptDirectoryFullPath;
    int stage;
    int frame;
    NativeList<Unity.Jobs.JobHandle> deleteCommentJobs;
    ScriptAnalyzeDataManager scriptAnalyzeDataManager;

    StringBuilder buffer;
    void Start()
    {
        stage = 0;
        frame = 0;
        rawScriptLoadReturnValue = new System.IO.DirectoryInfo(ScriptDirectoryFullPath).LoadFileToMemoryAsync(System.Text.Encoding.Unicode);
        scriptLoadReturnValue = new ScriptLoadReturnValue(ref rawScriptLoadReturnValue);
        // UnityEngine.Debug.Log("COUNT : " + rawScriptLoadReturnValue.Files.Length);
        deleteCommentJobs = new Unity.Collections.NativeList<Unity.Jobs.JobHandle>(rawScriptLoadReturnValue.Files.Length, Unity.Collections.Allocator.Persistent);
        for (int i = 0; i < rawScriptLoadReturnValue.FullPaths.Length; i++)
        {
            // UnityEngine.Debug.Log(i + " -> " + rawScriptLoadReturnValue.FullPaths[i] + "\n Length : " + rawScriptLoadReturnValue.Files[i].Length);
        }
        buffer = new StringBuilder(1024);
    }

    void OnDestroy()
    {
        scriptAnalyzeDataManager.Dispose();
        scriptLoadReturnValue.Dispose();
    }
    unsafe void Update()
    {
        switch (stage)
        {
            case 0:
                stage = ScriptFileLoader.TryConvertUnicodeAsync(ref rawScriptLoadReturnValue, ref scriptLoadReturnValue, ref deleteCommentJobs, false) ? 1 : 0;
                UnityEngine.Debug.Log("Frame : " + frame++);
                break;
            case 1:
                UnityEngine.Debug.Log("Frame : " + frame++);
                UnityEngine.Debug.Log("Done!");
                scriptAnalyzeDataManager = ScriptAnalyzeDataManager.Create(ref scriptLoadReturnValue);
                scriptAnalyzeDataManager.ParseStart();
                stage = 2;
                break;
            case 2:
                UnityEngine.Debug.Log("Frame : " + frame++);
                scriptAnalyzeDataManager.Update();
                if (scriptAnalyzeDataManager.CurrentStage == 3)
                    stage = 3;
                break;
            case 3:
                UnityEngine.Debug.Log("Frame : " + frame++);
                var d = scriptAnalyzeDataManager.RaceParserTempData;
                UnityEngine.Debug.Log(d.Length);
                for (int i = 0; i < d.Length; i++)
                {
                    buffer.Clear().Append(d.Values[i], scriptAnalyzeDataManager.Files, scriptAnalyzeDataManager.RaceParserTempData, scriptAnalyzeDataManager.IdentifierNumberPairList, scriptAnalyzeDataManager.ASTValueTypePairList).AppendLine();
                    UnityEngine.Debug.Log(buffer.ToString());
                }
                stage = 4;
                break;
        }
    }
    private unsafe void Debug(int index)
    {
        UnityEngine.Debug.Log(buffer.ToString());
    }
}