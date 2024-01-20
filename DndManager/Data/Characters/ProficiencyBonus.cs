namespace DndManager.Data.Characters;

public class ProficiencyBonus : EntityBase
{
    public int CharacterId { get; set; }

    public SkillType? SkillType { get; set; }

    public StatType? AbilityModifierType { get; set; }

    public virtual Character Character { get; set; } = null!;
}
