namespace pcysl5edgo.Wahren.AST
{
    public enum Location
    {
        None = 0,
        Power = 1,
        Unit = 2,
        Race = 3,
        Attribute = 4,
        Field = 5,
        Object = 6,
        Movetype = 7,
        Event = 8,
        Dungeon = 9,
        Detail = 10,
        Class = 11,
        Context = 12,
        Skill = 13,
        SkillSet = 14,
        Sound = 15,
        Story = 16,
        Scenario = 17,
        Spot = 18,
        Voice = 19,
        WorkSpace = 20,
    }
    public enum PendingReason
    {
        None,
        Other,
        ASTValueTypePairListCapacityShortage,
        IdentifierNumberPairListCapacityShortage,
        SentenceListCapacityShortage,
        TreeListCapacityShortage,
        SectionListCapacityShortage,
    }
}