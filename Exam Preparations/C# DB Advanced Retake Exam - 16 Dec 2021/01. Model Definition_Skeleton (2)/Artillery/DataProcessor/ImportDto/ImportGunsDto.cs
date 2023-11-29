using Artillery.Data.Models;
using Artillery.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Artillery.DataProcessor.ImportDto
{
    public class ImportGunsDto
    {
        public ImportGunsDto()
        {
            Countries = new HashSet<Country>();    
        }

        public int ManufacturerId { get; set; }

        [Range(100, 1350000)]
        public int GunWeight { get; set; }

        [Range(2.00, 35.00)]
        public double BarrelLength { get; set; }

        public int? NumberBuild { get; set; }

        [Range(1, 100000)]
        public int Range { get; set; }

        [Required]
        public string GunType { get; set; } = null!;

        public int ShellId { get; set; }

        public virtual ICollection<Country> Countries { get; set; } = null!;
    }
}
