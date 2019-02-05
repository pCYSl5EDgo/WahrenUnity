using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Wahren
{
    public static unsafe class ListUtility
    {
        public static void Lengthen<T>(ref T* ptr, ref int currentCapacity, Allocator allocator = Allocator.Persistent) where T : unmanaged
        {
            if (currentCapacity < 0) throw new ArgumentOutOfRangeException(nameof(currentCapacity) + " : " + currentCapacity);
            if (currentCapacity == 0) return;
            var _ = (T*)UnsafeUtility.Malloc(sizeof(T) * currentCapacity * 2, UnsafeUtility.AlignOf<T>(), allocator);
            UnsafeUtility.MemCpy(_, ptr, sizeof(T) * currentCapacity);
            UnsafeUtility.Free(ptr, allocator);
            ptr = _;
            currentCapacity *= 2;
        }

        public static void AddToTempJob<T>(this ref T value, ref T* values, ref int capacity, ref int length, out int index) where T : unmanaged
        {
            if (capacity == 0)
            {
                capacity = 2;
                values = (T*)UnsafeUtility.Malloc(sizeof(T) * capacity, 4, Allocator.Temp);
                values[0] = value;
                index = 0;
                length = 1;
                return;
            }
            if (capacity == length)
            {
                capacity *= 2;
                var tmps = (T*)UnsafeUtility.Malloc(sizeof(T) * capacity, 4, Allocator.Temp);
                UnsafeUtility.MemCpy(tmps, values, sizeof(T) * length);
                UnsafeUtility.Free(values, Allocator.Temp);
                values = tmps;
            }
            values[index = length++] = value;
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
        public static T* MallcTemp<T>(int capacity, out int length) where T : unmanaged
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
    }
}