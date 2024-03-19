namespace CampaignManager.DataContracts.Initiatives;

public record UpdateCombatantDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

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

