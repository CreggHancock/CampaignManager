namespace DndManager.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Language
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public required string Name { get; set; }

    public virtual Character Character { get; set; } = null!;
}
