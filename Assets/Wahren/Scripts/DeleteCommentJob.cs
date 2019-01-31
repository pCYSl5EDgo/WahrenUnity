using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    [BurstCompile]
    public unsafe struct DeleteCommentJob : IJobParallelFor
    {
        public byte isDebug;
        [NativeDisableUnsafePtrRestriction] public ushort** Lines;
        [NativeDisableUnsafePtrRestriction] public int* LineLengths;

        public static (bool, JobHandle) Schedule(TextFile file, bool isDebug, int innerloopBatchCount) => file.Length == 0 ? (false, default) :
            (true, new DeleteCommentJob
            {
                isDebug = isDebug ? (byte)1 : (byte)0,
                Lines = (ushort**)file.Lines,
                LineLengths = file.LineLengths,
            }.Schedule(file.LineCount, innerloopBatchCount));

        public DeleteCommentJob(ref TextFile file, bool isDebug)
        {
            this.Lines = (ushort**)file.Lines;
            this.LineLengths = file.LineLengths;
            this.isDebug = (byte)(isDebug ? 1 : 0);
        }

        public void Execute(int index)
        {
            var cptr = Lines[index];
            for (int i = 0, length = LineLengths[index], state = 0; i < length; ++i, ++cptr)
            {
                switch (state)
                {
                    case 0:
                        if (*cptr == '/')
                            state = 1;
                        break;
                    case 1:
                        if (*cptr == '/')
                        {
                            LineLengths[index] = i - 1;
                            return;
                        }
                        if (*cptr == '+')
                        {
                            if (isDebug == 0)
                            {
                                LineLengths[index] = i - 1;
                                return;
                            }
                            *(cptr - 1) = *cptr = ' ';
                        }
                        state = 0;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}