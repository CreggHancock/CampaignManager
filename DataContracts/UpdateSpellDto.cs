namespace DndManager.DataContracts;

public record UpdateSpellDto
{
	public int Id { get; set; }

	public int CharacterId { get; set; }

	public int Level { get; set; }

	public required string Name { get; set; }
}
