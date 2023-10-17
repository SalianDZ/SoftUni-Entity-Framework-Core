using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string logoUrl { get; set; }

        [Required]
        [MaxLength(4)]
        public string Initials { get; set; }
        public decimal Budget { get; set; }

        //Todo
        public int PrimaryKitColorId { get; set; }

        public int SecondaryKitColorId { get; set; }

        public int TownId { get; set; }
    }
}