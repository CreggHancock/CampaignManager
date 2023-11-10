namespace DndManager.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Ability
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public int MaxUses { get; set; }

    public int RemainingUses { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public virtual Character Character { get; set; } = null!;
}
