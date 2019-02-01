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
    }
}