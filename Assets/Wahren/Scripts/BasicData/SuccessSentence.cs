using System.Collections.ObjectModel;

namespace pcysl5edgo.Wahren.AST
{
    using static SuccessSentence.Kind;
    public static class SuccessSentence
    {
        public static readonly ReadOnlyDictionary<Kind, string> Contents;
        public enum Kind
        {
            None,
            StructKindInterpretSuccess,
            StructNameInterpretSuccess,
            ParentStructNameInterpretSuccess,
            ScenarioVariantInterpretSuccess,
            RaceTreeIntrepretSuccess,
            AssignmentInterpretationSuccess,
            LeftBraceConfirmationSuccess,
            NumberInterpretSuccess,
            IdentifierInterpretSuccess,
            MovetypeTreeInterpretSuccess,
            VoiceTreeIntrepretSuccess,
            SentencesEndWithSemicolonInterpretSuccess,
        }

        static SuccessSentence()
        {
            var d = new System.Collections.Generic.Dictionary<Kind, string>()
            {
                { None, "No Successful Token" },
                { StructKindInterpretSuccess, "Struct Kind Tag" },
                { StructNameInterpretSuccess, "Struct Name" },
                { ParentStructNameInterpretSuccess, "Parent Struct Name" },
                { ScenarioVariantInterpretSuccess, "Scenario Variant Name Interpretation Success" },
                { RaceTreeIntrepretSuccess, "Struct 'race' Interpretation Success" },
                { AssignmentInterpretationSuccess, "Assignment Interpretation Success" },
                { LeftBraceConfirmationSuccess, "'{' was Confirmed" },
                { NumberInterpretSuccess, "Number Interpretation Success" },
                { IdentifierInterpretSuccess, "Identifier Interpretation Success" },
                { MovetypeTreeInterpretSuccess, "Struct 'movetype' Interpretation Success" },
                { VoiceTreeIntrepretSuccess, "Struct 'voice' Interpretation Success" },
                { SentencesEndWithSemicolonInterpretSuccess, "Sentences Interpretation Success" },
            };
            Contents = new ReadOnlyDictionary<Kind, string>(d);
        }
    }
}