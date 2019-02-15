using NUnit.Framework;
using pcysl5edgo.Wahren.AST;

using static UnityEngine.Assertions.Assert;

public unsafe class UnitTest
{
    void InitializeComponents(char* ptr, int length, out System.Text.StringBuilder buffer, out TextFile file, out InterpreterStatus token, out ParseJob.CommonData cdata, out ScriptAnalyzeDataManager_Internal script)
    {
        file = default;
        token = default;
        cdata = default;
        script = default;
        fixed (TextFile* fp = &file)
        fixed (InterpreterStatus* tp = &token)
        fixed (ParseJob.CommonData* cp = &cdata)
        fixed (ScriptAnalyzeDataManager_Internal* sp = &script)
        {
            InitializeComponents(ptr, length, out buffer, fp, tp, cp, sp);
        }
    }
    void InitializeComponents(char* ptr, int length, out System.Text.StringBuilder buffer, TextFile* file, InterpreterStatus* token, ParseJob.CommonData* cdata, ScriptAnalyzeDataManager_Internal* script)
    {
        buffer = new System.Text.StringBuilder(256);
        *file = new TextFile(0, length);
        *token = InterpreterStatus.None;
        *cdata = new ParseJob.CommonData
        {
            Caret = new Caret(),
            LastNameSpan = default,
            LastParentNameSpan = default,
            LastStructKind = Location.None,
            Result = new TryInterpretReturnValue(new Span(), 0, 0, InterpreterStatus.None)
        };
        *script = new ScriptAnalyzeDataManager_Internal
        {
            ASTValueTypePairList = new ASTValueTypePairList(4),
            FileLength = 1,
            Files = file,
            MoveTypeParserTempData = new MovetypeParserTempData(1),
            RaceParserTempData = new RaceParserTempData(1),
        };
        file->Contents = (ushort*)ptr;
        file->Split();
        var job = new ParseJob
        {
            CancellationTokenPtr = token,
            CommonPtr = cdata,
            File = *file,
            ScriptPtr = script,
        };
        ref var result = ref cdata->Result;
    PARSE:
        job.Execute();
        AreNotEqual(result.Status, InterpreterStatus.Error);
        if (result.Status == InterpreterStatus.Pending)
        {
            var (location, reason) = result;
            AreEqual(location, Location.Race);
            AreNotEqual(reason, PendingReason.Other);
            script->RaceParserTempData.Lengthen(ref script->ASTValueTypePairList, result, false);
            goto PARSE;
        }
    }
    [Test]
    public void UnitTestSimplePasses()
    {
        var str0 = "o0_21uruse021902e";
        var str1 = "絶対に許さねえ！ドン・サウザンド！";
        var scriptText = $@"race {str0}{{
            name = {str1}
        }}";
        fixed (char* ptr = scriptText)
        {
            InitializeComponents(ptr, scriptText.Length, out var buffer, out var file, out var token, out var cdata, out var script);
            AreEqual(script.RaceParserTempData.Length, 1);
            ref var tree = ref script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual(type, RaceTree.name);
            AreEqual(script.RaceParserTempData.NameLength, 1);
            var nameAssignExpression = script.RaceParserTempData.Names[value];
            AreEqual(nameAssignExpression.ScenarioVariant.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(file, nameAssignExpression.Value).ToString(), str1);
            file.Contents = null;
            file.Dispose();
            script.Files = null;
            script.Dispose();
        }
    }
}