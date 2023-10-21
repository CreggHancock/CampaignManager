using DndManager.Data;

namespace DndManager.Models;

public class CharacterSheetViewModel
{
    public required Character Character { get; init; } = null!;
}
