using System.Threading;

namespace pcysl5edgo.Wahren.AST
{
    public unsafe struct ASTValueTypePair
    {
        public int Value;
        public int Type;

        public ASTValueTypePair(int value, int type)
        {
            this.Value = value;
            this.Type = type;
        }

        public ASTValueTypePair(int type)
        {
            this.Value = default;
            this.Type = type;
        }

        public void Deconstruct(out int value, out int type)
        {
            value = Value;
            type = Type;
        }

        public bool TryAddAST<T>(T* list, in T value, int capacity, ref int length) where T : unmanaged
        {
            do
            {
                if (capacity == length)
                {
                    return false;
                }
                Value = length;
            } while (Value != Interlocked.CompareExchange(ref length, Value + 1, Value));
            list[Value] = value;
            return true;
        }
    }
}