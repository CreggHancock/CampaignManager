namespace DndManager.Data.Characters;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Spell
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public int Level { get; set; }

    public bool IsCustom { get; set; }

    public required string Name { get; set; }

    public virtual Character Character { get; set; } = null!;
}
