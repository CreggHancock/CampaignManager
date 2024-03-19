namespace CampaignManager.Data.Characters;

public class Language : EntityBase
{
    public bool IsCustom { get; set; }

    public required string Name { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = Array.Empty<Character>();
}
