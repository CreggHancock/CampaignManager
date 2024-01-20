namespace DndManager.Data.Characters;

public class CharacterClass : EntityBase
{
    public int CharacterId { get; set; }

    public int ClassDefinitionId { get; set; }

    public int Level { get; set; }

    public virtual Character Character { get; set; } = null!;

    public virtual ClassDefinition ClassDefinition { get; set; } = null!;
}
