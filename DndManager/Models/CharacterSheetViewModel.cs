using DndManager.Data.Characters;

namespace DndManager.Models;

public class CharacterSheetViewModel
{
    public required Character Character { get; init; } = null!;
}
