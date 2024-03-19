using CampaignManager.Data.Characters;

namespace CampaignManager.DataContracts.Characters;

public record UpdateProficiencyBonusDto
{
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public SkillType? SkillType { get; set; }

    public StatType? AbilityModifierType { get; set; }
}
