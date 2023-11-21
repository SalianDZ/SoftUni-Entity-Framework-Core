using Boardgames.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellersDto
    {
        public ImportSellersDto()
        {
            Boardgames= new List<int>();    
        }

        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Address { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        public string Website { get; set; } = null!;

        public List<int> Boardgames { get; set; } = null!;
    }
}
