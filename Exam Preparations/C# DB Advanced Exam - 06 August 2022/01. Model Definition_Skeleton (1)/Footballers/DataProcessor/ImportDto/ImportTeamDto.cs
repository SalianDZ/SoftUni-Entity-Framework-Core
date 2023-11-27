using System.ComponentModel.DataAnnotations;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamDto
    {
        public ImportTeamDto()
        {
            Footballers = new HashSet<int>();    
        }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Nationality { get; set; } = null!;

        public int Trophies { get; set; }

        public ICollection<int> Footballers { get; set; }
    }
}
