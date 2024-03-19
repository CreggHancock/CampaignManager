namespace CampaignManager.DataContracts.Characters;

public record UpdateCharacterClassDto
{
    public int Id { get; set; }

    public int CharacterId { get; set; }

    public required string Name { get; set; }

    public int Level { get; set; }
}
