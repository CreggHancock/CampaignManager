namespace CampaignManager.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class EntityBase
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }
}