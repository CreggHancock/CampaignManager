namespace CampaignManager.Data.Characters;

public class SubClass : EntityBase
{
    public int ClassId { get; set; }

    public int SubClassDefinitionId { get; set; }

    public virtual CharacterClass CharacterClass { get; set; } = null!;

    public virtual SubClassDefinition SubClassDefinition { get; set; } = null!;
}