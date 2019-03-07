using NUnit.Framework;
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
        file.Split(allocator);
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
        script = new ScriptAnalyzeDataManager_Internal(1, allocator);
        script.Files[0] = file;
        fixed (ScriptAnalyzeDataManager_Internal* scriptPtr = &script)
        {
            new ParseJob(&token, scriptPtr, file, &cdata, allocator).Execute();
        }
        commonData = cdata;
    }
    public void Dispose()
    {
        script.Dispose(this.allocator);
    }
}
