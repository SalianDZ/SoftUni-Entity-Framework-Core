using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicHub.Data.Models;

public class Album
{
    [Key]
    public int Id { get; set; }

    [MaxLength(40)]
    public string Name { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    //The sum of all songs in the album
    public decimal Price
    {
        get
        {
            if (Songs != null)
            {
                return Songs.Sum(song => song.Price);
            }
            return 0; // Return 0 if there are no songs or if Songs is null
        }
    }

    [ForeignKey(nameof(Producer))]
    public int? ProducerId { get; set; }

    public virtual Producer Producer { get; set; }

    //A collection of songs

    public virtual ICollection<Song> Songs { get; set; } = null!;
}

