using System;
using System.IO;
using System.Text;
using Unity.IO.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public unsafe static class ScriptFileLoader
    {
        public static bool TryConvertUnicodeAsync(ref RawScriptLoadReturnValue rawScriptLoadReturnValue, ref ScriptLoadReturnValue scriptLoadReturnValue)
        {
            var invalidCounter = 0;
            for (int i = 0; i < rawScriptLoadReturnValue.Handles.Length; i++)
            {
                var handle = rawScriptLoadReturnValue.Handles[i];
                if (handle.IsValid())
                {
                    switch (handle.Status)
                    {
                        case ReadStatus.Complete:
                            handle.Dispose();
                            scriptLoadReturnValue.Files[i] = TextFile.FromRawTextFileUtf16(rawScriptLoadReturnValue.Files[i]);
                            invalidCounter++;
                            break;
                        case ReadStatus.InProgress:
                            continue;
                        case ReadStatus.Failed:
                            UnityEngine.Debug.LogError(rawScriptLoadReturnValue.FullPaths[i]);
                            handle.Dispose();
                            break;
                    }
                }
                else invalidCounter++;
            }
            if (invalidCounter != rawScriptLoadReturnValue.Handles.Length)
                return false;
            rawScriptLoadReturnValue.Files.Dispose();
            rawScriptLoadReturnValue.DisposeExceptFiles();
            return true;
        }

        public static bool TryConvertCp932Async(ref RawScriptLoadReturnValue rawScriptLoadReturnValue, ref ScriptLoadReturnValue scriptLoadReturnValue)
        {
            var cp932 = Encoding.GetEncoding(932);
            var invalidCounter = 0;
            for (int i = 0; i < rawScriptLoadReturnValue.Handles.Length; i++)
            {
                var handle = rawScriptLoadReturnValue.Handles[i];
                if (handle.IsValid())
                {
                    switch (handle.Status)
                    {
                        case ReadStatus.Complete:
                            handle.Dispose();
                            scriptLoadReturnValue.Files[i] = TextFile.FromRawTextFileOtherEncoding(rawScriptLoadReturnValue.Files[i], cp932);
                            invalidCounter++;
                            break;
                        case ReadStatus.InProgress:
                            continue;
                        case ReadStatus.Failed:
                            UnityEngine.Debug.LogError(rawScriptLoadReturnValue.FullPaths[i]);
                            handle.Dispose();
                            break;
                    }
                }
                else invalidCounter++;
            }
            if (invalidCounter != rawScriptLoadReturnValue.Handles.Length)
                return false;
            rawScriptLoadReturnValue.Dispose();
            return true;
        }

        public static RawScriptLoadReturnValue LoadFileToMemoryAsync(this DirectoryInfo scriptDirectoryInfo, Encoding encoding)
        {
            if (scriptDirectoryInfo is null) throw new ArgumentNullException(nameof(scriptDirectoryInfo));
            if (!scriptDirectoryInfo.Exists) throw new DirectoryNotFoundException(scriptDirectoryInfo.FullName);
            if (encoding is null) throw new ArgumentNullException(nameof(encoding));
            var fileInfos = scriptDirectoryInfo.GetFiles("*.dat", SearchOption.AllDirectories);
            var answer = new RawScriptLoadReturnValue(fileInfos.Length);
            var cmdListPtr = (ReadCommand*)NativeListUnsafeUtility.GetUnsafePtr(answer.Commands);
            if (object.ReferenceEquals(encoding, Encoding.Unicode))
                LoadAsync(fileInfos, ref answer, cmdListPtr, 2);
            else
                LoadAsync(fileInfos, ref answer, cmdListPtr, 0);
            return answer;
        }
        private static void LoadAsync(FileInfo[] fileInfos, ref RawScriptLoadReturnValue answer, ReadCommand* cmdListPtr, int offset)
        {
            for (int i = 0; i < fileInfos.Length; i++)
            {
                var file = fileInfos[i];
                var length = file.Length - offset;
                answer.Names[i] = file.Name;
                answer.FullPaths[i] = file.FullName;
                if (length <= 0)
                    DontLoad(ref answer, i);
                else
                    Load(ref answer, cmdListPtr, offset, i, length);
            }
        }

        private static void Load(ref RawScriptLoadReturnValue answer, ReadCommand* cmdListPtr, int offset, int index, long length)
        {
            answer.Files[index] = new RawTextFile(index, length);
            answer.Commands.Add(new ReadCommand
            {
                Buffer = (void*)answer.Files[index].Contents,
                Offset = offset,
                Size = length,
            });
            answer.Handles.Add(AsyncReadManager.Read(answer.FullPaths[index], cmdListPtr + (answer.Commands.Length - 1), 1));
        }

        private static void DontLoad(ref RawScriptLoadReturnValue answer, int index) => answer.Files[index] = new RawTextFile
        {
            FilePathId = index,
            Length = 0,
            Contents = null,
        };
    }
}
