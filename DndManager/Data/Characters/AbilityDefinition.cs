namespace DndManager.Data.Characters;

public class AbilityDefinition : EntityBase
{
    public int MaxUses { get; set; }

    public bool IsCustom { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public virtual ICollection<CharacterAbility> CharacterAbilities { get; set; } = Array.Empty<CharacterAbility>();
}
