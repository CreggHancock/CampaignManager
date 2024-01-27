using DndManager.Data.Characters;
using DndManager.Data.Initiatives;

namespace DndManager.Models;

public class HomeViewModel
{
    public required IEnumerable<Character> UserCharacters { get; init; }

    public required IEnumerable<Scene> UserScenes { get; init; }
}
