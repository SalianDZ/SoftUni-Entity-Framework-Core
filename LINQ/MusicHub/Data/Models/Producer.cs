using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models;

public class Producer
{
    [Key]
    public int Id { get; set; }

    [MaxLength(30)]
    public string Name { get; set; } = null!;

    public string? Pseudonym { get; set; }

    public string? PhoneNumber { get; set; }

    //A collection of albums is needed here!!

    public virtual ICollection<Album> Albums { get; set; } = null!;
}

