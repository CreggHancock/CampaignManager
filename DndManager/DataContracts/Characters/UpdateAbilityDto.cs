namespace DndManager.DataContracts.Characters;

public record UpdateAbilityDto
{
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public int MaxUses { get; set; }

    public int RemainingUses { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }
}
