using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren.AST
{
    public static unsafe class ListUtility
    {
        public static void MemMove<T>(ref T* ptr, ref int listLength, int destinationIndex, int sourceIndex) where T : unmanaged
        {
            UnsafeUtility.MemMove(ptr + destinationIndex, ptr + sourceIndex, (listLength - sourceIndex) * sizeof(T));
            listLength -= sourceIndex - destinationIndex;
        }

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

        public static bool TryAddToMultiThread<T>(this ref T value, ref T* values, int capacity, ref int length, out int index) where T : unmanaged
        {
            do
            {
                index = length;
                if (index == capacity)
                    return false;
            } while (index != Interlocked.CompareExchange(ref length, index + 1, index));
            values[index] = value;
            return true;
        }
        public static bool TryAddBulkToMultiThread<T>(T* srcValues, int srcLength, T* destValues, ref int destLength, int destCapacity, out int start) where T : unmanaged
        {
            do
            {
                start = destLength;
                if (start + srcLength > destCapacity)
                    return false;
            } while (start != Interlocked.CompareExchange(ref destLength, start + srcLength, start));
            UnsafeUtility.MemCpy(destValues + start, srcValues, srcLength * sizeof(T));
            return true;
        }
        public static bool TryAddBulkToMultiThread<T>(T* srcValues, long srcLength, T* destValues, ref long destLength, long destCapacity, out long start) where T : unmanaged
        {
            do
            {
                start = destLength;
                if (start + srcLength > destCapacity)
                    return false;
            } while (start != Interlocked.CompareExchange(ref destLength, start + srcLength, start));
            UnsafeUtility.MemCpy(destValues + start, srcValues, srcLength * sizeof(T));
            return true;
        }
        public static T* MallocTemp<T>(int capacity, out int length) where T : unmanaged
        {
            length = 0;
            return (T*)UnsafeUtility.Malloc(sizeof(T) * capacity, 4, Allocator.Temp);
        }
        public static T* MallocTemp<T>(long capacity, out long length) where T : unmanaged
        {
            length = 0;
            return (T*)UnsafeUtility.Malloc(sizeof(T) * capacity, 4, Allocator.Temp);
        }
        public static void FreeTemp<T>(ref T* values, ref int capacity, ref int length) where T : unmanaged
        {
            if (capacity != 0)
                UnsafeUtility.Free(values, Allocator.Temp);
            length = 0;
            values = null;
            capacity = 0;
        }
        public static void FreeTemp<T>(ref T* values, ref long capacity, ref long length) where T : unmanaged
        {
            if (capacity != 0)
                UnsafeUtility.Free(values, Allocator.Temp);
            length = 0;
            values = null;
            capacity = 0;
        }
    }
}