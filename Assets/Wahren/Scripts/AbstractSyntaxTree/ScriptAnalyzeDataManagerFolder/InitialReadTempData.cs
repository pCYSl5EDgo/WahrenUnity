using System;
using System.Text;
using System.IO;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IO.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    partial class ScriptAnalyzeDataManager
    {
        private unsafe struct InitialReadTempData
        {
            private RawTextFile* RawFiles;
            private TextFile** FilesPtr;
            private ReadHandle* ReadHandles;
            private ReadCommand* ReadCommands;
            private JobHandle* DeleteCommentJobHandles;
            private int _isUtf16, _isDebug;
            public bool IsUtf16 => _isUtf16 == 2;
            public bool IsDebug => _isDebug != 0;
            public enum Stage
            {
                None, ReadAsync, DeleteCommentAsync, Done,
            }
            private Stage _currentStage;
            public Stage CurrentStage => _currentStage;
            private int Length;

            private const Allocator allocator = Allocator.Persistent;

            public void Complete()
            {
                switch (_currentStage)
                {
                    case Stage.None:
                    case Stage.Done:
                        return;
                    case Stage.ReadAsync:
                        ReadComplete();
                        goto case Stage.DeleteCommentAsync;
                    case Stage.DeleteCommentAsync:
                        DeleteCommentComplete();
                        return;
                }
            }

            private void DeleteCommentComplete()
            {
                for (int i = 0; i < Length; i++)
                {
                    DeleteCommentJobHandles[i].Complete();
                }
                UnsafeUtility.Free(DeleteCommentJobHandles, allocator);
                DeleteCommentJobHandles = null;
                _currentStage = Stage.Done;
            }

            private void ReadComplete()
            {
                for (int i = 0; i < Length; i++)
                {
                    if (!ReadHandles[i].IsValid()) continue;
                    ReadHandles[i].JobHandle.Complete();
                    ReadHandles[i].Dispose();
                    if (IsUtf16)
                    {
                        FilesPtr[0][i] = TextFile.FromRawTextFileUtf16(RawFiles[i], allocator);
                        DeleteCommentJobHandles[i] = DeleteCommentJob.Schedule(FilesPtr[0] + i, IsDebug);
                    }
                    else
                    {
                        FilesPtr[0][i] = TextFile.FromRawTextFileCp932(RawFiles[i], allocator);
                        DeleteCommentJobHandles[i] = DeleteCommentJob.Schedule(FilesPtr[0] + i, IsDebug);
                    }
                }
                _currentStage = Stage.DeleteCommentAsync;
            }

            public void Update()
            {
                switch (_currentStage)
                {
                    case Stage.None:
                    case Stage.Done:
                        return;
                    case Stage.DeleteCommentAsync:
                        DeleteCommentAsyncUpdate();
                        return;
                    case Stage.ReadAsync:
                        ReadAsyncUpdate();
                        return;
                    default:
                        throw new InvalidOperationException();
                }
            }

            private void DeleteCommentAsyncUpdate()
            {
                bool isAnyRunning = false;
                for (int i = 0; i < Length; i++)
                {
                    if (!DeleteCommentJobHandles[i].IsCompleted)
                    {
                        isAnyRunning = true;
                        continue;
                    }
                    DeleteCommentJobHandles[i].Complete();
                }
                if (isAnyRunning) return;
                if (DeleteCommentJobHandles != null)
                {
                    UnsafeUtility.Free(DeleteCommentJobHandles, allocator);
                    DeleteCommentJobHandles = null;
                }
                _currentStage = Stage.Done;
            }

            private void ReadAsyncUpdate()
            {
                bool isAnyRunning = false;
                for (int i = 0; i < Length; i++)
                {
                    if (!ReadHandles[i].IsValid())
                        continue;
                    switch (ReadHandles[i].Status)
                    {
                        case ReadStatus.Complete:
                            ReadHandles[i].Dispose();
                            if (IsUtf16)
                            {
                                FilesPtr[0][i] = TextFile.FromRawTextFileUtf16(RawFiles[i], allocator);
                                DeleteCommentJobHandles[i] = DeleteCommentJob.Schedule(FilesPtr[0] + i, IsDebug);
                            }
                            else
                            {
                                FilesPtr[0][i] = TextFile.FromRawTextFileCp932(RawFiles[i], allocator);
                                DeleteCommentJobHandles[i] = DeleteCommentJob.Schedule(FilesPtr[0] + i, IsDebug);
                            }
                            break;
                        case ReadStatus.Failed:
                            ReadHandles[i].Dispose();
                            break;
                        case ReadStatus.InProgress:
                            isAnyRunning = true;
                            break;
                    }
                }
                if (isAnyRunning) return;
                _currentStage = Stage.DeleteCommentAsync;
                if (RawFiles != null)
                {
                    if (IsUtf16)
                    {
                        UnsafeUtility.Free(RawFiles, allocator);
                    }
                    else
                    {
                        for (int i = 0; i < Length; i++)
                        {
                            RawFiles[i].Dispose();
                        }
                        UnsafeUtility.Free(RawFiles, allocator);
                    }
                    RawFiles = null;
                }
                if (ReadHandles != null)
                {
                    UnsafeUtility.Free(ReadHandles, allocator);
                    ReadHandles = null;
                }
                if (ReadCommands != null)
                {
                    UnsafeUtility.Free(ReadCommands, allocator);
                    ReadCommands = null;
                }
            }

            public void StartLoad(FileInfo[] infos, string[] fullPaths)
            {
                _currentStage = Stage.ReadAsync;
                for (int i = 0; i < Length; i++)
                {
                    RawFiles[i] = new RawTextFile(i, infos[i].Length - _isUtf16);
                    ReadCommands[i] = new ReadCommand
                    {
                        Size = RawFiles[i].Length,
                        Buffer = RawFiles[i].Contents,
                        Offset = _isUtf16,
                    };
                    ReadHandles[i] = AsyncReadManager.Read(fullPaths[i], ReadCommands + i, 1);
                }
            }

            public static InitialReadTempData* CreatePtr(FileInfo[] infos, int fileLength, TextFile** filesPtr, bool isUtf16, bool isDebug)
            {
                var ptr = (InitialReadTempData*)UnsafeUtility.Malloc(sizeof(InitialReadTempData), 4, allocator);
                *ptr = new InitialReadTempData
                {
                    _isDebug = isDebug ? 1 : 0,
                    _isUtf16 = isUtf16 ? 2 : 0,
                    _currentStage = Stage.None,
                    Length = fileLength,
                    RawFiles = (RawTextFile*)UnsafeUtility.Malloc(sizeof(RawTextFile) * fileLength, 4, allocator),
                    FilesPtr = filesPtr,
                    ReadHandles = (ReadHandle*)UnsafeUtility.Malloc(sizeof(ReadHandle) * fileLength, 4, allocator),
                    ReadCommands = (ReadCommand*)UnsafeUtility.Malloc(sizeof(ReadCommand) * fileLength, 4, allocator),
                    DeleteCommentJobHandles = (JobHandle*)UnsafeUtility.Malloc(sizeof(JobHandle) * fileLength, 4, allocator),
                };
                return ptr;
            }

            public void Dispose()
            {
                if (DeleteCommentJobHandles != null)
                {
                    UnsafeUtility.Free(DeleteCommentJobHandles, allocator);
                }
                if (RawFiles != null)
                {
                    if (IsUtf16)
                    {
                        UnsafeUtility.Free(RawFiles, allocator);
                    }
                    else
                    {
                        for (int i = 0; i < Length; i++)
                        {
                            RawFiles[i].Dispose();
                        }
                        UnsafeUtility.Free(RawFiles, allocator);
                    }
                }
                if (ReadHandles != null)
                {
                    UnsafeUtility.Free(ReadHandles, allocator);
                }
                if (ReadCommands != null)
                {
                    UnsafeUtility.Free(ReadCommands, allocator);
                }
                this = default;
            }
        }
    }
}