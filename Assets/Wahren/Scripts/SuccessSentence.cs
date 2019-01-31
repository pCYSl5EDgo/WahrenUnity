using System;

public static class SuccessSentence
{
    public static readonly string[] Contents;
    public static readonly string[][] SubContents;

    internal const int StructKindInterpretSuccess = 1;
    internal const int StructNameInterpretSuccess = 2;
    internal const int ParentStructNameInterpretSuccess = 3;
    static SuccessSentence()
    {
        Contents = new string[]
        {
            "No Successful Token",
            "Struct Kind Tag",
            "Struct Name",
            "Parent Struct Name",
        };
        SubContents = new string[][]
        {
            Array.Empty<string>(),
            new string[]{
                "'power'", // 0
                "'unit'",
                "'race'", // 2
                "'attribute'",
                "'field'", //4
                "'object'",
                "'movetype'", // 6
                "'event'",
                "'dungeon'", // 8
                "'detail'",
                "'class'", // 10
                "'context'",
                "'skill'", // 12
                "'skillset'",
                "'sound'", // 14
                "'story'",
                "'scenario'", // 16
                "'spot'", // 17
            },
        };
    }
}
