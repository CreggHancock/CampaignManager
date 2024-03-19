using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CampaignManager.DataContracts.Characters;

public record UpdateCharacterDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int Level { get; set; }

    public int Health { get; set; }

    public int MaxHealth { get; set; }

    public int TempHealth { get; set; }

    public int Gold { get; set; }

    public int Silver { get; set; }

    public int Copper { get; set; }

    public int Electrum { get; set; }

    public int Platinum { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Background { get; set; } = string.Empty;

    public string Race { get; set; } = string.Empty;

    public string Alignment { get; set; } = string.Empty;

    public int ExperiencePoints { get; set; }

    public int InitiativeBonus { get; set; }

    public int ProficiencyBonus { get; set; }

    public int Speed { get; set; }

    public string HitDice { get; set; } = string.Empty;

    public bool HasInspiration { get; set; }

    public int Strength { get; set; }

    public int Dexterity { get; set; }

    public int Constitution { get; set; }

    public int Intelligence { get; set; }

    public int Wisdom { get; set; }

    public int Charisma { get; set; }
}
