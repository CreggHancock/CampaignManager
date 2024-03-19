namespace CampaignManager.Data.Characters;

public class CharacterAbility : EntityBase
{
    public int CharacterId { get; set; }

    public int AbilityDefinitionId { get; set; }

    public int RemainingUses { get; set; }

    public virtual Character Character { get; set; } = null!;

    public virtual AbilityDefinition AbilityDefinition { get; set; } = null!;
}
