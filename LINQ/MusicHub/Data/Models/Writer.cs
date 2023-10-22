using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models;

public class Writer
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    public string Name { get; set; } = null!;

    public string? Pseudonym { get; set; }

    //A collection of songs is needed here!!
    public virtual ICollection<Song> Songs { get; set; } = null!;
}

