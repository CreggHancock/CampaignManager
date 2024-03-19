namespace CampaignManager.Data.Characters;

public class ClassDefinition : EntityBase
{
    public required string Name { get; set; }

    public int Level { get; set; }

    public bool IsCustom { get; set; }

    public virtual ICollection<CharacterClass> CharacterClasses { get; set; } = Array.Empty<CharacterClass>();

    public virtual ICollection<SubClassDefinition> SubSubClassDefinitions { get; set; } = Array.Empty<SubClassDefinition>();
}
