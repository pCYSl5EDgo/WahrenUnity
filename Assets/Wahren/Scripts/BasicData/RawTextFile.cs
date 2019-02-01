using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public unsafe struct RawTextFile : IDisposable
    {
        public int FilePathId;
        public long Length;
        public byte* Contents;

        public bool IsCreated => Contents != null;

        public RawTextFile(int filePathId, long length)
        {
            FilePathId = filePathId;
            Length = length;
            Contents = (byte*)UnsafeUtility.Malloc(Length, 1, Allocator.Persistent);
        }

        public static RawTextFile CreateEmptyFile(int filePathId) => new RawTextFile
        {
            FilePathId = filePathId,
            Length = 0,
            Contents = null,
        };

        public void Dispose()
        {
            if (IsCreated)
            {
                UnsafeUtility.Free(Contents, Allocator.Persistent);
                this = default;
            }
        }
    }
}