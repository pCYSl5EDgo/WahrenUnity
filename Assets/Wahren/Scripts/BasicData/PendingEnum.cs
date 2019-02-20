namespace pcysl5edgo.Wahren.AST
{
    public enum Location
    {
        None = -1,
        Power = 0,
        Unit = 1,
        Race = 2,
        Attribute = 3,
        Field = 4,
        Object = 5,
        Movetype = 6,
        Event = 7,
        Dungeon = 8,
        Detail = 9,
        Class = 10,
        Context = 11,
        Skill = 12,
        SkillSet = 13,
        Sound = 14,
        Story = 15,
        Scenario = 16,
        Spot = 17,
        Voice = 18,
        WorkSpace = 19,
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