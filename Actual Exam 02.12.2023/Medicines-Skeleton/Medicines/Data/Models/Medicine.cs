using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medicines.Data.Models
{
    public class Medicine
    {
        public Medicine()
        {
            PatientsMedicines = new HashSet<PatientMedicine>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public DateTime ProductionDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Producer { get; set; } = null!;

        public int PharmacyId { get; set; }

        [ForeignKey(nameof(PharmacyId))]
        public virtual Pharmacy Pharmacy { get; set; } = null!;

        public virtual ICollection<PatientMedicine> PatientsMedicines { get; set; }
    }
}
