﻿using NUnit.Framework;
using pcysl5edgo.Wahren.AST;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using static UnityEngine.Assertions.Assert;

unsafe struct USING_STRUCT : System.IDisposable
{
    public TextFile file;
    public ScriptAnalyzeDataManager_Internal script;
    public ParseJob.CommonData commonData;
    private Allocator allocator;
    public USING_STRUCT(string input, Allocator allocator, out System.Text.StringBuilder buffer)
    {
        this.allocator = allocator;
        file = new TextFile(0, input.Length, allocator);
        fixed (char* p = input)
        {
            UnsafeUtility.MemCpy(file.Contents, p, input.Length * sizeof(char));
        }
        file.Split();
        buffer = new System.Text.StringBuilder(256);
        var token = InterpreterStatus.None;
        var cdata = new ParseJob.CommonData
        {
            Caret = new Caret(),
            LastNameSpan = default,
            LastParentNameSpan = default,
            LastStructKind = Location.None,
            Result = new TryInterpretReturnValue(new Span(), 0, 0, InterpreterStatus.None)
        };
        fixed (ScriptAnalyzeDataManager_Internal* scriptPtr = &script)
        {
            script = new ScriptAnalyzeDataManager_Internal
            {
                ASTValueTypePairList = new ASTTypePageIndexPairList(4, allocator),
                FileLength = 1,
                Files = (TextFile*)UnsafeUtility.Malloc(sizeof(System.IntPtr), 4, allocator),
                MovetypeParserTempData = new MovetypeParserTempData(1, allocator),
                RaceParserTempData = new RaceParserTempData(1, allocator),
            };
            *script.Files = file;
            var job = new ParseJob
            {
                CancellationTokenPtr = &token,
                CommonPtr = &cdata,
                File = file,
                ScriptPtr = scriptPtr,
            };
            ref var result = ref cdata.Result;
        PARSE:
            job.Execute();
            if (result.Status == InterpreterStatus.Pending)
            {
                var (location, reason) = result;
                switch (location)
                {
                    case Location.Race:
                        script.RaceParserTempData.Lengthen(ref script.ASTValueTypePairList, result, false);
                        break;
                    case Location.Movetype:
                        script.MovetypeParserTempData.Lengthen(ref script.ASTValueTypePairList, result, false);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
                result = default;
                token = default;
                goto PARSE;
            }
        }
        commonData = cdata;
    }
    public void Dispose()
    {
        script.Dispose(this.allocator);
    }
}
