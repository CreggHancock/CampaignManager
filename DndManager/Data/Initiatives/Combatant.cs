namespace DndManager.Data.Initiatives;

public class Combatant : EntityBase
{
    public required string UserId { get; set; }

    public int SceneId { get; set; }

    public required string Name { get; set; }

    public string? Image { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int LocationX { get; set; }

    public int LocationY { get; set; }

    public bool IsPlayer { get; set; }

    public int Health { get; set; }

    public int MaxHealth { get; set; }

    public int InitiativeModifier { get; set; }

    public int HardInitiative { get; set; }
}
