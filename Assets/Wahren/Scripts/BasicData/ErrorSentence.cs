
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
        internal const int NotExpectedIdentifierAfterStructNameError = 7;
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
                "':' or '{' are expected. Invalid Character Found", // 7
            };
            SubContents = new string[][] {
                null,
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
                    "'voice'", // 22
                    "'workspace'", // 23
                },
                null, // 2
                null, // 3
                null, // 4
                null, // 5
                null, // 6
                null, // 7
            };
        }
    }
}
