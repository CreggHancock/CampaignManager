namespace DndManager.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SpellSlot
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public int Level { get; set; }

    public int RemainingUses { get; set; }

    public int MaxUses { get; set; }

    public virtual Character Character { get; set; } = null!;
}
