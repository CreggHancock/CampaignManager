namespace DndManager.DataContracts;

public class UpdateLanguageDto
{
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public required string Name { get; set; }
}
