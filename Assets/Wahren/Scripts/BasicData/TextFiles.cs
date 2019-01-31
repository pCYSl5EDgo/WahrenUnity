using System;
using System.Text;
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

    public unsafe struct TextFile : IDisposable
    {
        public int FilePathId;
        public int Length;
        public char* Contents;
        public char** Lines;
        public int* LineLengths;
        public int LineCount;

        public override string ToString() => new string(Contents, 0, Length);
        public string ToStringUnsafe(int lineIndex) => new string(Lines[lineIndex], 0, LineLengths[lineIndex]);
        public string ToString(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= LineCount) throw new ArgumentOutOfRangeException();
            if (Lines == null) throw new NullReferenceException();
            return new string(Lines[lineIndex], 0, LineLengths[lineIndex]);
        }
        public string ToString(Span span) => new string(Lines[span.Line], span.Column, span.Length);

        public TextFile(int filePathId, int length)
        {
            FilePathId = filePathId;
            Length = length;
            Contents = (char*)UnsafeUtility.Malloc(sizeof(char) * length, 1, Allocator.Persistent);
            Lines = null;
            LineLengths = null;
            LineCount = 0;
        }

        public static TextFile CreateEmptyFile(int filePathId) => new TextFile
        {
            FilePathId = filePathId,
        };

        public static TextFile FromRawTextFileUtf16(RawTextFile file)
        {
            var answer = new TextFile
            {
                Contents = (char*)file.Contents,
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
                encoding.GetChars(file.Contents, (int)file.Length, answer.Contents, answer.Length);
                answer.Split();
                return answer;
            }
            return CreateEmptyFile(file.FilePathId);
        }

        internal void Split()
        {
            if (!IsCreated) return;
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
            Lines = (char**)UnsafeUtility.Malloc(sizeof(IntPtr) * LineCount, 4, Allocator.Persistent);
            Lines[0] = Contents;
            LineLengths = (int*)UnsafeUtility.Malloc(sizeof(int) * LineCount, 4, Allocator.Persistent);
            UnsafeUtility.MemClear(LineLengths, sizeof(int) * LineCount);
            for (int i = 0, lineIndex = 0; i < Length; i++)
            {
                switch (Contents[i])
                {
                    case '\r':
                        break;
                    case '\n':
                        Lines[++lineIndex] = Contents + i + 1;
                        break;
                    default:
                        ++LineLengths[lineIndex];
                        break;
                }
            }
        }

        public bool IsCreated => Contents != null;

        public void Dispose()
        {
            if (IsCreated)
            {
                UnsafeUtility.Free(Contents, Allocator.Persistent);
                if (LineLengths != null)
                    UnsafeUtility.Free(LineLengths, Allocator.Persistent);
                if (Lines != null)
                    UnsafeUtility.Free(Lines, Allocator.Persistent);
                this = default;
            }
        }
    }
}