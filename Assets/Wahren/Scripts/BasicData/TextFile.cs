﻿using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct TextFile
    {
        public int FilePathId;
        public int Length;
        [NativeDisableUnsafePtrRestriction] public ushort* Contents;
        [NativeDisableUnsafePtrRestriction] public int* LineLengths;
        [NativeDisableUnsafePtrRestriction] public int* LineStarts;
        public int LineCount;

        public TextFile(int filePathId, int length, Allocator allocator)
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
                Contents = (ushort*)UnsafeUtility.Malloc(sizeof(char) * length, 1, allocator);
            }
        }

        public static TextFile FromRawTextFileUtf16(RawTextFile file, Allocator allocator)
        {
            var answer = new TextFile
            {
                Contents = (ushort*)file.Contents,
                Length = (int)(file.Length / 2),
                FilePathId = file.FilePathId,
            };
            answer.Split(allocator);
            return answer;
        }

        public static TextFile FromRawTextFileCp932(RawTextFile file, Allocator allocator)
        {
            if (file.IsCreated)
            {
                var rawFileLength = (ulong)file.Length;
                var answer = new TextFile(file.FilePathId, (int)pcysl5edgo.BurstEncoding.Cp932Decoder.GetCharCount(file.Contents, rawFileLength), Allocator.Persistent);
                BurstEncoding.Cp932Decoder.GetChars(file.Contents, rawFileLength, answer.Contents);
                answer.Split(allocator);
                return answer;
            }
            return new TextFile
            {
                FilePathId = file.FilePathId,
            };
        }

        internal void Split(Allocator allocator)
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
            LineStarts = (int*)UnsafeUtility.Malloc(size, 4, allocator);
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

        public void Dispose(Allocator allocator)
        {
            if (Contents != null)
                UnsafeUtility.Free(Contents, allocator);
            if (LineStarts != null)
                UnsafeUtility.Free(LineStarts, allocator);
            this = default;
        }
    }
}