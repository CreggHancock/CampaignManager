using CampaignManager.Data.Characters;

namespace CampaignManager.Models;

public class CharacterSheetViewModel
{
    public required Character Character { get; init; } = null!;
}
