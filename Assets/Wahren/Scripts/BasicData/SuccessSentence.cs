using System;

public static class SuccessSentence
{
    public static readonly string[] Contents;
    public static readonly string[][] SubContents;
    internal const int StructKindInterpretSuccess = 1;
    internal const int StructNameInterpretSuccess = 2;
    internal const int ParentStructNameInterpretSuccess = 3;
    internal const int ScenarioVariantInterpretSuccess = 4;
    internal const int RaceTreeIntrepretSuccess = 5;
    internal const int AssignmentInterpretationSuccess = 6;
    internal const int LeftBraceConfirmationSuccess = 7;
    internal const int NumberInterpretSuccess = 8;
    internal const int IdentifierInterpretSuccess = 9;
    internal const int MovetypeTreeInterpretSuccess = 10;
    internal const int VoiceTreeIntrepretSuccess = 11;

    static SuccessSentence()
    {
        Contents = new string[]
        {
            "No Successful Token",
            "Struct Kind Tag",
            "Struct Name", // 2
            "Parent Struct Name",
            "Scenario Variant Name Interpretation Success", // 4
            "Struct 'race' Interpretation Success", //5
            "Assignment Interpretation Success", // 6
            "'{' was Confirmed", // 7
            "Number Interpretation Success", // 8
            "Identifier Interpretation Success", // 9
            "Struct 'movetype' Interpretation Success", //10
            "Struct 'voice' Interpretation Success", // 11
        };
        SubContents = new string[][]
        {
            null,
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
                "'voice'", // 18
                "'workspace'", // 19
            },
            null, // 2
            null,
            null, // 4
            null,
            new string[]
            {
                "'name'",
            },
            null, // 7
            null, // 8
            null, // 9
            null, // 10
            null, // 11
        };
    }
}
