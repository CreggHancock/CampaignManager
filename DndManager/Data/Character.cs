namespace DndManager.Data;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Character
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    public required string UserId { get; set; }

    public int Level { get; set; }

    public int Health { get; set; }

    public int MaxHealth { get; set; }

    public int TempHealth { get; set; }

    public int Gold { get; set; }

    public int Silver { get; set; }

    public int Copper { get; set; }

    public int Electrum { get; set; }

    public int Platinum { get; set; }

    [StringLength(255)]
    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Race { get; set; }

    public required string Alignment { get; set; }

    public int ExperiencePoints { get; set; }

    public int InitiativeBonus { get; set; }

    public int ProficiencyBonus { get; set; }

    public int Speed { get; set; }

    public required string HitDice { get; set; }

    public bool HasInspiration { get; set; }

    public int Strength { get; set; }

    public int Dexterity { get; set; }

    public int Constitution { get; set; }

    public int Intelligence { get; set; }

    public int Wisdom { get; set; }

    public int Charisma { get; set; }

    public ICollection<CharacterClass> CharacterClasses { get; set; } = Array.Empty<CharacterClass>();

    public ICollection<Ability> Abilities { get; set; } = Array.Empty<Ability>();

    public ICollection<InventoryItem> InventoryItems { get; set; } = Array.Empty<InventoryItem>();

    public ICollection<ProficiencyBonus> ProficiencyBonuses { get; set; } = Array.Empty<ProficiencyBonus>();

    public ICollection<Spell> Spells { get; set; } = Array.Empty<Spell>();

    public ICollection<SpellSlot> SpellSlots { get; set; } = Array.Empty<SpellSlot>();
}
