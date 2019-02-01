using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public static class EqualityComparerUtility
    {
        public static unsafe bool Equals(this in ScriptLoadReturnValue script, in Span span0, in Span span1)
        {
            if (span0.Length != span1.Length)
                return false;
            char* ptr0 = script.Files[span0.File].Lines[span0.Line] + span0.Column;
            char* ptr1 = script.Files[span1.File].Lines[span1.Line] + span1.Column;
            return UnsafeUtility.MemCmp(ptr0, ptr1, sizeof(char) * span0.Length) == 0;
        }
    }
}