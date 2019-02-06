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
                UnityEngine.Debug.Log("Done!");
                scriptAnalyzeDataManager = ScriptAnalyzeDataManager.Create(ref scriptLoadReturnValue);
                stage = 2;
                break;
            case 2:
                for (int i = 0; i < scriptAnalyzeDataManager.Length; i++)
                {
                    if (i == 5)
                        Debug(i);
                }
                stage = 3;
                break;
        }
    }
    private unsafe void Debug(int index)
    {
        var file = scriptAnalyzeDataManager[index];
        var value0 = file.TryGetFirstStructLocation(default);
        TryInterpretReturnValue nameResult, parentNameResult, value3, value4;
        nameResult = parentNameResult = value3 = value4 = new TryInterpretReturnValue { Span = new Span { File = index } };
        Caret caret = value0.Span.CaretNextToEndOfThisSpan;
        file.SkipWhiteSpace(ref caret);
        if (StructAnalyzer.IsStructKindWithName(value0.SubDataIndex))
        {
            nameResult = file.TryGetStructName(caret);
            caret = nameResult.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref caret);
            if (file.TryGetParentStructName(caret, out parentNameResult))
            {
                caret = parentNameResult.Span.CaretNextToEndOfThisSpan;
                file.SkipWhiteSpace(ref caret);
            }
        }
        value3 = file.IsCurrentCharEquals(caret, '{');
        caret = value3.Span.CaretNextToEndOfThisSpan;
        file.SkipWhiteSpace(ref caret);
        switch (value0.SubDataIndex)
        {
            case 2:
                value4 = file.TryParseRaceStructMultiThread(ref scriptAnalyzeDataManager.RaceParserTempData, ref scriptAnalyzeDataManager.IdentifierNumberPairList, ref scriptAnalyzeDataManager.ASTValueTypePairList, nameResult.Span, parentNameResult.Span, caret, out var nextToRightBrace, out int raceTreeIndex);
                var raceTree = scriptAnalyzeDataManager.RaceParserTempData.Values[raceTreeIndex];
                if (value4.IsSuccess)
                {
                    buffer.Append(raceTree, scriptAnalyzeDataManager.Files, scriptAnalyzeDataManager.RaceParserTempData, scriptAnalyzeDataManager.IdentifierNumberPairList, scriptAnalyzeDataManager.ASTValueTypePairList);
                    file.SkipWhiteSpace(ref nextToRightBrace);
                }
                else if (value4.IsPending)
                {
                    buffer.Append("Pending");
                }
                else if (value4.IsError)
                {
                    buffer.Append(value4, scriptAnalyzeDataManager);
                }
                break;
            default:
                break;
        }
        UnityEngine.Debug.Log(buffer.ToString());
    }
}