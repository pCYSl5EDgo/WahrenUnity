using NUnit.Framework;
using pcysl5edgo.Wahren.AST;

using static UnityEngine.Assertions.Assert;

unsafe struct USING_STRUCT : System.IDisposable
{
    public TextFile file;
    public ScriptAnalyzeDataManager_Internal script;
    public ParseJob.CommonData commonData;
    public USING_STRUCT(string input, out System.Text.StringBuilder buffer)
    {
        file = new TextFile(0, input.Length);
        fixed (char* p = input)
        {
            Unity.Collections.LowLevel.Unsafe.UnsafeUtility.MemCpy(file.Contents, p, input.Length * sizeof(char));
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
                ASTValueTypePairList = new ASTValueTypePairList(4),
                FileLength = 1,
                Files = (TextFile*)Unity.Collections.LowLevel.Unsafe.UnsafeUtility.Malloc(sizeof(System.IntPtr), 4, Unity.Collections.Allocator.Persistent),
                MoveTypeParserTempData = new MovetypeParserTempData(1),
                RaceParserTempData = new RaceParserTempData(1),
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
                script.RaceParserTempData.Lengthen(ref script.ASTValueTypePairList, result, false);
                goto PARSE;
            }
        }
        commonData = cdata;
    }
    public void Dispose()
    {
        script.Dispose();
    }
}

public unsafe class UnitTest
{
    [Test]
    public void race_name_success()
    {
        var str0 = "o0_21uruse021902e";
        var str1 = "絶対に許さねえ！ドン・サウザンド！";
        var scriptText = $@"race {str0}{{
            name = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.name);
            AreEqual(_.script.RaceParserTempData.NameLength, 1);
            var nameAssignExpression = _.script.RaceParserTempData.Names[value];
            AreEqual(nameAssignExpression.ScenarioVariant.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, nameAssignExpression.Value).ToString(), str1);
        }
    }

    [Test]
    public void race_name_fail()
    {
        var str0 = "o0_21uruse021902e";
        var str1 = "絶対に許さねえ！\nドン・サウザンド！";
        var scriptText = $@"race {str0}{{
            name = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Race);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(_.commonData.Result.Span, new Span(0, 2, 0, 1));
        }
    }
}