﻿using MusicHub.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicHub.Data.Models;
public class Song
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    public string Name { get; set; } = null!;

    public TimeSpan Duration { get; set; }

    public DateTime CreatedOn { get; set; }

    public Genre Genre { get; set; }

    //Foreign key
    [ForeignKey(nameof(Album))]
    public int? AlbumId { get; set; }

    public virtual Album Album { get; set; }

    //Foreign key
    [ForeignKey(nameof(Writer))]
    public int WriterId { get; set; }

    public virtual Writer Writer { get; set; } = null!;

    public decimal Price { get; set; }

    //Mapping table collection

    public virtual ICollection<SongPerformer> SongPerformers { get; set; } = null!;
}

