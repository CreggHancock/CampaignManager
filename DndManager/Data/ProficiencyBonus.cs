namespace DndManager.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProficiencyBonus
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public SkillType? SkillType { get; set; }

    public StatType? AbilityModifierType { get; set; }

    public virtual Character Character { get; set; } = null!;
}
