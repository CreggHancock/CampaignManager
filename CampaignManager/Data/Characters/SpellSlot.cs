namespace CampaignManager.Data.Characters;

public class SpellSlot : EntityBase
{
    public int CharacterId { get; set; }

    public int Level { get; set; }

    public int RemainingUses { get; set; }

    public int MaxUses { get; set; }

    public virtual Character Character { get; set; } = null!;
}
