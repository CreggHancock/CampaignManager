using DndManager.Data.Characters;

namespace DndManager.DataContracts.Characters;

public record UpdateProficiencyBonusDto
{
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public SkillType? SkillType { get; set; }

    public StatType? AbilityModifierType { get; set; }
}
