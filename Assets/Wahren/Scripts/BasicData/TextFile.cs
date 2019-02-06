﻿using System;
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
        [NativeDisableUnsafePtrRestriction] public ushort** Lines;
        [NativeDisableUnsafePtrRestriction] public int* LineLengths;
        public int LineCount;

        public TextFile(int filePathId, int length)
        {
            FilePathId = filePathId;
            Length = length;
            Contents = (ushort*)UnsafeUtility.Malloc(sizeof(char) * length, 1, Allocator.Persistent);
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
            Lines = (ushort**)UnsafeUtility.Malloc(sizeof(IntPtr) * LineCount, 4, Allocator.Persistent);
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