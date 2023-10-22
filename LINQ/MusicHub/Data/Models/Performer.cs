using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models;

public class Performer
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    public string FirstName { get; set; } = null!;

    [MaxLength(20)]
    public string LastName { get; set; } = null!;

    public int Age { get; set; }

    public decimal NetWorth { get; set; }

    //A collection of the mapping table

    public virtual ICollection<SongPerformer> PerformerSongs { get; set; } = null!;
}

