using DndManager.Data;
using System.Text.Json.Serialization;

namespace DndManager.Controllers;

public record UpdateCharacterDto
{
    public required Character Character { get; set; }
}
