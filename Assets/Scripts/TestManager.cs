﻿using UnityEngine;

using pcysl5edgo.Wahren;

public class TestManager : MonoBehaviour
{
    RawScriptLoadReturnValue rawScriptLoadReturnValue;
    ScriptLoadReturnValue scriptLoadReturnValue;
    [SerializeField] public string ScriptDirectoryFullPath;
    int stage;
    int frame;
    Unity.Collections.NativeList<Unity.Jobs.JobHandle> deleteCommentJobs;
    void Start()
    {
        stage = 0;
        frame = 0;
        rawScriptLoadReturnValue = new System.IO.DirectoryInfo(ScriptDirectoryFullPath).LoadFileToMemoryAsync(System.Text.Encoding.Unicode);
        scriptLoadReturnValue = new ScriptLoadReturnValue(ref rawScriptLoadReturnValue);
        UnityEngine.Debug.Log("COUNT : " + rawScriptLoadReturnValue.Files.Length);
        deleteCommentJobs = new Unity.Collections.NativeList<Unity.Jobs.JobHandle>(rawScriptLoadReturnValue.Files.Length, Unity.Collections.Allocator.Persistent);
        for (int i = 0; i < rawScriptLoadReturnValue.FullPaths.Length; i++)
        {
            UnityEngine.Debug.Log(rawScriptLoadReturnValue.FullPaths[i] + "\n Length : " + rawScriptLoadReturnValue.Files[i].Length);
        }
    }

    void OnDestroy()
    {
        scriptLoadReturnValue.Dispose();
    }
    void Update()
    {
        switch (stage)
        {
            case 0:
                stage = ScriptFileLoader.TryConvertUnicodeAsync(ref rawScriptLoadReturnValue, ref scriptLoadReturnValue, ref deleteCommentJobs, false) ? 1 : 0;
                UnityEngine.Debug.Log("Frame : " + frame++);
                break;
            case 1:
                UnityEngine.Debug.Log("Done!");
                for (int i = 0; i < scriptLoadReturnValue.Files.Length; i++)
                {
                    var file = scriptLoadReturnValue.Files[i];
                    var value = file.TryGetFirstStructLocationUnsafe(default);
                    UnityEngine.Debug.Log(value.ToString(ref scriptLoadReturnValue));
                }
                stage = 2;
                break;
        }
    }
}