using UnityEngine;

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
        // for (int i = 0; i < rawScriptLoadReturnValue.FullPaths.Length; i++)
        // {
        //     UnityEngine.Debug.Log(rawScriptLoadReturnValue.FullPaths[i] + "\n Length : " + rawScriptLoadReturnValue.Files[i].Length);
        // }
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
                for (int i = 0; i < scriptLoadReturnValue.FullPaths.Length; i++)
                {
                    Debug(i);
                }
                stage = 2;
                break;
        }
    }
    private void Debug(int index)
    {
        var file = scriptLoadReturnValue.Files[index];
        var value0 = file.TryGetFirstStructLocation(default);
        UnityEngine.Debug.Log("================");
        UnityEngine.Debug.Log(value0.ToString(ref scriptLoadReturnValue));
        var span = value0.Span;
        if (StructAnalyzer.IsStructKindWithName(value0.SubDataIndex))
        {
            UnityEngine.Debug.Log("----------------");
            var value1 = file.TryGetStructName(span.CaretNextToEndOfThisSpan);
            UnityEngine.Debug.Log(value1.ToString(ref scriptLoadReturnValue));
            if (file.TryGetParentStructName(value1.Span.CaretNextToEndOfThisSpan, out var value2))
            {
                UnityEngine.Debug.Log(value2.ToString(ref scriptLoadReturnValue));
                span = value2.Span;
            }
            UnityEngine.Debug.Log("----------------");
        }
    }
}