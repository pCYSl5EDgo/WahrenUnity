namespace pcysl5edgo.Wahren.AST
{
    public enum PendingLocation
    {
        None,
        Power,
        Unit,
        Race,
        Attribute,
        Field,
        Object,
        Movetype,
        Event,
        Dungeon,
        Detail,
        Class,
        Context,
        Skill,
        SkillSet,
        Sound,
        Story,
        Scenario,
        Spot,
        Voice,
        WorkSpace,
    }
    public enum PendingReason
    {
        None,
        Other,
        ASTValueTypePairListCapacityShortage,
        IdentifierNumberPairListCapacityShortage,
        TreeListCapacityShortage,
        SectionListCapacityShortage,
    }
}