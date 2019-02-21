using System.Collections.ObjectModel;

namespace pcysl5edgo.Wahren.AST
{
    using static ErrorSentence.Kind;
    public static class ErrorSentence
    {
        public static readonly ReadOnlyDictionary<Kind, string> Contents;
        public enum Kind
        {
            None,
            StructKindInterpretError,
            StructKindNotFoundError,
            StructNameNotFoundError,
            ParentStructNameNotFoundError,
            IdentifierCannotBeNumberError,
            InvalidIdentifierError,
            NotExpectedCharacterError,
            ExpectedCharNotFoundError,
            NotNumberError,
            InvalidEndOfLineError,
            InvalidMinusNumberError,
            OutOfRangeError,
        }
        static ErrorSentence()
        {
            Contents = new ReadOnlyDictionary<Kind, string>(new System.Collections.Generic.Dictionary<Kind, string>()
            {
                { None, "No Error, Success!" },
                { StructKindInterpretError,"Expects struct kind"},
                { StructKindNotFoundError,"No Struct Kind Tag Found"},
                { StructNameNotFoundError,"No Struct Name Found"},
                { ParentStructNameNotFoundError,"Parent Struct Name Not Found"},
                { IdentifierCannotBeNumberError,"Identifier Should NOT be a Number"},
                { InvalidIdentifierError,"Invalid Identifier"},
                { NotExpectedCharacterError,"Invalid Character"},
                { ExpectedCharNotFoundError,"Expected Character Not Found"},
                { NotNumberError,"Expects Number but Found Identifier"},
                { InvalidEndOfLineError,"Invalid End of Line"},
                { InvalidMinusNumberError,"Minus number should consist of '-' and digits"},
                { OutOfRangeError,"Out of Range Error"},
            });
        }
    }
}
