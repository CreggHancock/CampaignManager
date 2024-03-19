namespace CampaignManager.Data.Initiatives;

public class Scene : EntityBase
{
    public required string UserId { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public string? Image { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int? SquareSize { get; set; }

    public int CombatantTurn { get; set; }

    public int Round { get; set; }

    public virtual ICollection<Combatant> Combatants { get; set; } = Array.Empty<Combatant>();
}
