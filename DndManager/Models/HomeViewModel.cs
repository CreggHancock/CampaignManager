using DndManager.Data.Characters;

namespace DndManager.Models;

public class HomeViewModel
{
    public bool IsLoggedIn { get; init; }

    public required IEnumerable<Character> UserCharacters { get; init; }
}
