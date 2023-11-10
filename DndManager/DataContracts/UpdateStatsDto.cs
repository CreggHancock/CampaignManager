namespace DndManager.DataContracts;

public record UpdateStatsDto
{
	public int Id { get; set; }

	public int CharacterId { get; set; }

	public int Level { get; set; }

	public int Strength { get; set; }

	public int Dexterity { get; set; }

	public int Constitution { get; set; }

	public int Intelligence { get; set; }

	public int Wisdom { get; set; }

	public int Charisma { get; set; }
}
