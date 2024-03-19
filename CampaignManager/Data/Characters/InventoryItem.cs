namespace CampaignManager.Data.Characters;

public class InventoryItem : EntityBase
{
    public int CharacterId { get; set; }

    public required string Name { get; set; }

    public virtual Character Character { get; set; } = null!;
}
