
namespace pcysl5edgo.Wahren
{
    internal static class ErrorSentence
    {
        internal static readonly string[] Contents;
        internal static readonly string[][] SubContents;
        internal const int StructKindInterpretError = 1;
        internal const int StructKindNotFoundError = 2;
        internal const int StructNameNotFoundError = 3;
        internal const int ParentStructNameNotFoundError = 4;
        internal const int IdentifierCannotBeNumberError = 5;
        internal const int InvalidIdentifierError = 6;
        static ErrorSentence()
        {
            Contents = new string[]
            {
                "No Error, Success!",
                "Expects struct kind",
                "No Struct Kind Tag Found", // 2
                "No Struct Name Found",
                "Parent Struct Name Not Found", // 4
                "Identifier Should NOT be a Number",
                "Invalid Identifier", // 6
            };
            SubContents = new string[][] {
                System.Array.Empty<string>(),
                new string[]
                {
                    "'power'",
                    "'unit'",
                    "'race'",
                    "'attribute'",
                    "'field'", //4
                    "'object'",
                    "'movetype'", // 6
                    "'event'",
                    "'dungeon'", // 8
                    "'detail'",
                    "'detail' or 'dungeon'", // 10
                    "'class'",
                    "'context'", // 12
                    "'class' or 'context'",
                    "'skill'", //14
                    "'skillset'",
                    "'skill' or 'skillset'", //16
                    "'sound'",
                    "'story'", //18
                    "'scenario'",
                    "'spot', skill', 'sound', 'story', 'scenario' or 'skillset'", //20
                    "'spot'", //21
                },
                System.Array.Empty<string>(), // 2
                System.Array.Empty<string>(), // 3
                System.Array.Empty<string>(), // 4
                System.Array.Empty<string>(), // 5
                System.Array.Empty<string>(), // 6
            };
        }
    }
}
