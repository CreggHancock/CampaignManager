namespace DndManager.Data.Characters;

public class SubClassDefinition : EntityBase
{
    public int ClassDefinitionId { get; set; }

    public required string Name { get; set; }

    public bool IsCustom { get; set; }

    public virtual ICollection<SubClass> SubClasses { get; set; } = Array.Empty<SubClass>();

    public virtual ClassDefinition ClassDefinition { get; set; } = null!;
}