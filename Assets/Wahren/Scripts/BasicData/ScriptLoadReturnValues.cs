using System;
using Unity.Jobs;
using Unity.IO.LowLevel.Unsafe;
using Unity.Collections;

namespace pcysl5edgo.Wahren
{
    public struct RawScriptLoadReturnValue : IDisposable
    {
        public NativeList<ReadHandle> Handles;
        public NativeList<ReadCommand> Commands;
        public string[] Names;
        public string[] FullPaths;
        public NativeArray<RawTextFile> Files;

        public bool IsCreated { get; private set; }

        public RawScriptLoadReturnValue(int capacity)
        {
            Handles = new NativeList<ReadHandle>(capacity, Allocator.Persistent);
            Commands = new NativeList<ReadCommand>(capacity, Allocator.Persistent);
            Names = new string[capacity];
            FullPaths = new string[capacity];
            Files = new NativeArray<RawTextFile>(capacity, Allocator.Persistent);
            IsCreated = true;
        }

        public void Dispose()
        {
            if (IsCreated)
            {
                DisposeExceptFiles();
                if (Files.IsCreated)
                {
                    for (int i = 0; i < Files.Length; i++)
                        Files[i].Dispose();
                    Files.Dispose();
                }
                IsCreated = false;
            }
        }
        public void DisposeExceptFiles()
        {
            if (IsCreated)
            {
                if (Handles.IsCreated)
                {
                    for (int i = 0; i < Handles.Length; i++)
                        if (Handles[i].IsValid())
                            Handles[i].Dispose();
                    Handles.Dispose();
                }
                if (Commands.IsCreated)
                {
                    Commands.Dispose();
                }
                Names = null;
                FullPaths = null;
                IsCreated = false;
            }
        }
    }

    public struct ScriptLoadReturnValue : IDisposable
    {
        public string[] Names;
        public string[] FullPaths;
        public NativeArray<TextFile> Files;
        private bool isCreated;

        public bool IsCreated => isCreated;


        public ScriptLoadReturnValue(ref RawScriptLoadReturnValue value)
        {
            this.Names = value.Names;
            this.FullPaths = value.FullPaths;
            this.Files = new NativeArray<TextFile>(value.Files.Length, Allocator.Persistent);
            this.isCreated = true;
        }

        public unsafe string ToString(ref Span span) => new string((char*)Files[span.File].Lines[span.Line], span.Column, span.Length);

        public void Dispose()
        {
            if (IsCreated)
            {
                Names = null;
                FullPaths = null;
                if (Files.IsCreated)
                {
                    for (int i = 0; i < Files.Length; i++)
                        Files[i].Dispose();
                    Files.Dispose();
                    Files = default;
                }
                isCreated = false;
            }
        }
    }
}