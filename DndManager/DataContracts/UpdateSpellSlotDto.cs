namespace DndManager.DataContracts;

public record UpdateSpellSlotDto
{
	public int Id { get; set; }

	public int CharacterId { get; set; }

	public int Level { get; set; }

	public int RemainingUses { get; set; }

	public int MaxUses { get; set; }
}
