namespace DndManager.DataContracts;

public record UpdateInventoryItemDto
{
	public int Id { get; set; }

	public int CharacterId { get; set; }

	public required string Name { get; set; }
}
