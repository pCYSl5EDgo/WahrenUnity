using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class ListUtility
    {
        public static void Lengthen<T>(ref T* ptr, ref int currentCapacity, Allocator allocator) where T : unmanaged
        {
            if (currentCapacity < 0) throw new ArgumentOutOfRangeException(nameof(currentCapacity) + " : " + currentCapacity);
            if (currentCapacity == 0) return;
            var _ = (T*)UnsafeUtility.Malloc(sizeof(T) * currentCapacity * 2, UnsafeUtility.AlignOf<T>(), allocator);
            UnsafeUtility.MemCpy(_, ptr, sizeof(T) * currentCapacity);
            UnsafeUtility.Free(ptr, allocator);
            ptr = _;
            currentCapacity *= 2;
        }
        public static void Lengthen(ref void* ptr, ref int currentCapacity, int size, Allocator allocator)
        {
            if (currentCapacity < 0) throw new ArgumentOutOfRangeException(nameof(currentCapacity) + " : " + currentCapacity);
            if (currentCapacity == 0) return;
            var _ = (void*)UnsafeUtility.Malloc(size * currentCapacity * 2, 4, allocator);
            UnsafeUtility.MemCpy(_, ptr, size * currentCapacity);
            UnsafeUtility.Free(ptr, allocator);
            ptr = _;
            currentCapacity *= 2;
        }
    }
}