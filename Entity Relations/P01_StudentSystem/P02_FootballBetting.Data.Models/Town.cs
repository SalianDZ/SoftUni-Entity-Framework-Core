using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models
{
    public class Town
    {
        [Key]
        public int TownId { get; set; }

        [Required]
        [MaxLength(58)]
        public string Name { get; set; }


        public int CountryId { get; set; }

        //TODO: Create navigation properties
    }
}
