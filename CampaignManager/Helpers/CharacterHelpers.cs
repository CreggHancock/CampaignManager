using CampaignManager.Data.Characters;

namespace CampaignManager.Helpers;

public class CharacterHelpers
{
    public const int EmptyId = 0;

    public static Character BuildEmptyCharacter(string? userId)
    {
        if (userId == null)
        {
            throw new InvalidOperationException("Can't use an invalid user ID");
        }

        return new Character()
        {
            Id = EmptyId,
            UserId = userId,
            Name = string.Empty,
            Description = string.Empty,
            Background = string.Empty,
            Alignment = string.Empty,
            Race = string.Empty,
            HitDice = string.Empty,
        };
    }
}
