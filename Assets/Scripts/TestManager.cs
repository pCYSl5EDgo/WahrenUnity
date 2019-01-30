using UnityEngine;

using pcysl5edgo.Wahren;

public class TestManager : MonoBehaviour
{
    RawScriptLoadReturnValue rawScriptLoadReturnValue;
    ScriptLoadReturnValue scriptLoadReturnValue;
    [SerializeField] public string ScriptDirectoryFullPath;
    int stage;
    int frame;
    void Start()
    {
        stage = 0;
        frame = 0;
        rawScriptLoadReturnValue = new System.IO.DirectoryInfo(ScriptDirectoryFullPath).LoadFileToMemoryAsync(System.Text.Encoding.Unicode);
        scriptLoadReturnValue = new ScriptLoadReturnValue(ref rawScriptLoadReturnValue);
        UnityEngine.Debug.Log("COUNT : " + rawScriptLoadReturnValue.Files.Length);
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
                stage = ScriptFileLoader.TryConvertUnicodeAsync(ref rawScriptLoadReturnValue, ref scriptLoadReturnValue) ? 1 : 0;
                UnityEngine.Debug.Log("Frame : " + frame++);
                break;
            case 1:
                UnityEngine.Debug.Log("Done!");
                var file = scriptLoadReturnValue.Files[0];
                var tryInterpretReturnValue = file.TryGetFirstStructLocationUnsafe(false, new Span
                {
                    File = 0,
                    Line = 0,
                    Column = 0,
                });
                UnityEngine.Debug.Log(tryInterpretReturnValue.ToString(ref scriptLoadReturnValue));
                stage = 2;
                break;
        }
    }
}