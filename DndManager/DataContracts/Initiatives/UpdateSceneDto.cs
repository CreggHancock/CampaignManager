namespace DndManager.DataContracts.Initiatives;

public record UpdateSceneDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Image { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int? SquareSize { get; set; }

    public int CombatantTurn { get; set; }

    public int Round { get; set; }
}
