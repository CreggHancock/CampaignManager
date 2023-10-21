using DndManager.Data;

namespace DndManager.Helpers;

public class CharacterHelpers
{
    public static Character BuildEmptyCharacter(string? userId)
    {
        if (userId == null)
        {
            throw new InvalidOperationException("Can't use an invalid user ID");
        }

        return new Character()
        {
            Id = -1,
            UserId = userId,
            Name = string.Empty,
            Description = string.Empty,
            Alignment = string.Empty,
            Race = string.Empty,
            HitDice = string.Empty,
            CharacterClasses = Array.Empty<CharacterClass>(),
            InventoryItems = Array.Empty<InventoryItem>(),
            ProficiencyBonuses = Array.Empty<ProficiencyBonus>(),
            Spells = Array.Empty<Spell>(),
            SpellSlots = Array.Empty<SpellSlot>(),
            Abilities = Array.Empty<Ability>(),
        };
    }
}
