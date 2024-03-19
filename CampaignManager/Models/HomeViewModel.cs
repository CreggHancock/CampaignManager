using CampaignManager.Data.Characters;
using CampaignManager.Data.Initiatives;

namespace CampaignManager.Models;

public class HomeViewModel
{
    public required IEnumerable<Character> UserCharacters { get; init; }

    public required IEnumerable<Scene> UserScenes { get; init; }
}
