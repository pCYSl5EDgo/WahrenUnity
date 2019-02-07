using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public unsafe struct TextFile : IDisposable
    {
        public int FilePathId;
        public int Length;
        [NativeDisableUnsafePtrRestriction] public ushort* Contents;
        [NativeDisableUnsafePtrRestriction] public int* LineLengths;
        [NativeDisableUnsafePtrRestriction] public int* LineStarts;
        public int LineCount;

        public TextFile(int filePathId, int length)
        {
            FilePathId = filePathId;
            Length = length;
            LineStarts = null;
            LineLengths = null;
            LineCount = 0;
            if (length == 0)
            {
                Contents = null;
            }
            else
            {
                Contents = (ushort*)UnsafeUtility.Malloc(sizeof(char) * length, 1, Allocator.Persistent);
            }
        }

        public static TextFile CreateEmptyFile(int filePathId) => new TextFile
        {
            FilePathId = filePathId,
        };

        public static TextFile FromRawTextFileUtf16(RawTextFile file)
        {
            var answer = new TextFile
            {
                Contents = (ushort*)file.Contents,
                Length = (int)(file.Length / 2),
                FilePathId = file.FilePathId,
            };
            answer.Split();
            return answer;
        }

        public static TextFile FromRawTextFileOtherEncoding(RawTextFile file, Encoding encoding)
        {
            if (file.IsCreated)
            {
                var answer = new TextFile(file.FilePathId, encoding.GetCharCount(file.Contents, (int)file.Length));
                encoding.GetChars(file.Contents, (int)file.Length, (char*)answer.Contents, answer.Length);
                answer.Split();
                return answer;
            }
            return CreateEmptyFile(file.FilePathId);
        }

        internal void Split()
        {
            if (Length == 0) return;
            LineCount = 1;
            for (int i = 0; i < Length; i++)
            {
                switch (Contents[i])
                {
                    case '\n':
                        ++LineCount;
                        break;
                }
            }
            int size = sizeof(int) * 2 * LineCount;
            LineStarts = (int*)UnsafeUtility.Malloc(size, 4, Allocator.Persistent);
            LineStarts[0] = 0;
            LineLengths = LineStarts + LineCount;
            UnsafeUtility.MemClear(LineStarts, size);
            for (int i = 0, lineIndex = 0; i < Length; i++)
            {
                switch (Contents[i])
                {
                    case '\r':
                        break;
                    case '\n':
                        LineStarts[++lineIndex] = i + 1;
                        break;
                    default:
                        ++LineLengths[lineIndex];
                        break;
                }
            }
        }

        public void Dispose()
        {
            if (Contents != null)
                UnsafeUtility.Free(Contents, Allocator.Persistent);
            if (LineStarts != null)
                UnsafeUtility.Free(LineStarts, Allocator.Persistent);
            this = default;
        }
    }
}