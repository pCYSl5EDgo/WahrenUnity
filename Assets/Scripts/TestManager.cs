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
    RaceParserTempData raceParserTempData;
    IdentifierNumberPairList identifierNumberPairList;

    StringBuilder buffer;
    void Start()
    {
        stage = 0;
        frame = 0;
        rawScriptLoadReturnValue = new System.IO.DirectoryInfo(ScriptDirectoryFullPath).LoadFileToMemoryAsync(System.Text.Encoding.Unicode);
        scriptLoadReturnValue = new ScriptLoadReturnValue(ref rawScriptLoadReturnValue);
        // UnityEngine.Debug.Log("COUNT : " + rawScriptLoadReturnValue.Files.Length);
        deleteCommentJobs = new Unity.Collections.NativeList<Unity.Jobs.JobHandle>(rawScriptLoadReturnValue.Files.Length, Unity.Collections.Allocator.Persistent);
        raceParserTempData = new RaceParserTempData(16);
        for (int i = 0; i < rawScriptLoadReturnValue.FullPaths.Length; i++)
        {
            // UnityEngine.Debug.Log(i + " -> " + rawScriptLoadReturnValue.FullPaths[i] + "\n Length : " + rawScriptLoadReturnValue.Files[i].Length);
        }
        buffer = new StringBuilder(1024);
        identifierNumberPairList = new IdentifierNumberPairList(1024);
    }

    void OnDestroy()
    {
        scriptLoadReturnValue.Dispose();
        raceParserTempData.Dispose();
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
                    if (i == 5)
                        Debug(i);
                }
                stage = 2;
                break;
        }
    }
    private unsafe void Debug(int index)
    {
        var file = scriptLoadReturnValue.Files[index];
        var value0 = file.TryGetFirstStructLocation(default);
        TryInterpretReturnValue nameResult, parentNameResult, value3, value4;
        nameResult = parentNameResult = value3 = value4 = new TryInterpretReturnValue { Span = new Span { File = index } };
        buffer.Clear().AppendLine(value0.ToString(scriptLoadReturnValue));
        Caret caret = value0.Span.CaretNextToEndOfThisSpan;
        file.SkipWhiteSpace(ref caret);
        if (StructAnalyzer.IsStructKindWithName(value0.SubDataIndex))
        {
            nameResult = file.TryGetStructName(caret);
            buffer.AppendLine(nameResult.ToString(scriptLoadReturnValue));
            caret = nameResult.Span.CaretNextToEndOfThisSpan;
            file.SkipWhiteSpace(ref caret);
            if (file.TryGetParentStructName(caret, out parentNameResult))
            {
                buffer.AppendLine(parentNameResult.ToString(scriptLoadReturnValue));
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
                value4 = file.TryParseRaceStructMultiThread(ref raceParserTempData, ref identifierNumberPairList, nameResult.Span, parentNameResult.Span, caret, out var nextToRightBrace, out var raceTree);
                if (value4.IsSuccess)
                {
                    buffer.Append(raceTree, (TextFile*)scriptLoadReturnValue.Files.GetUnsafePtr(), raceParserTempData, identifierNumberPairList);
                }
                else if (value4.IsPending)
                {
                    buffer.Append(value4.ToString(scriptLoadReturnValue));
                }
                else if (value4.IsError)
                {
                    UnityEngine.Debug.Log(value4.Span.ToString() + " " + file.CurrentChar(value4.Span.Start));
                    buffer.Append(value4.ToString(scriptLoadReturnValue));
                }
                raceTree.Dispose();
                break;
            default:
                break;
        }
        UnityEngine.Debug.Log(buffer.ToString());
    }
}