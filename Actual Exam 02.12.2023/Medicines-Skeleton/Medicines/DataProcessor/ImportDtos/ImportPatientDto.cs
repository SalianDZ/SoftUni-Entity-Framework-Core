using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientDto
    {
        public ImportPatientDto()
        {
            Medicines = new List<int>();
        }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [Range(0, 2)]
        public int AgeGroup { get; set; }

        [Required]
        [Range(0, 1)]
        public int Gender { get; set; }

        public ICollection<int> Medicines { get; set; }
    }
}
