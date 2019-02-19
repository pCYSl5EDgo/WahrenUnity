namespace pcysl5edgo.Wahren.AST
{
    public interface INameStruct
    {
        void SetNameAndParentName(Span name, Span parentName);
    }

    internal static class CreateNameTreeHelper
    {
        public static T Create<T>(Span name, Span parentName) where T : unmanaged, INameStruct
        {
            T answer = default;
            answer.SetNameAndParentName(name, parentName);
            return answer;
        }
    }
}