using CampaignManager.Data.Initiatives;

namespace CampaignManager.Helpers;

public class SceneHelpers
{
    public const int EmptyId = 0;

    public static Scene BuildEmptyScene(string? userId)
    {
        if (userId == null)
        {
            throw new InvalidOperationException("Can't use an invalid user ID");
        }

        return new Scene()
        {
            Id = EmptyId,
            UserId = userId,
            Name = string.Empty,
            Description = string.Empty,
            Width = 100,
            Height = 100,
            SquareSize = 5
        };
    }
}
