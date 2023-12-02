using System.ComponentModel.DataAnnotations.Schema;

namespace Medicines.Data.Models
{
    public class PatientMedicine
    {
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public virtual Patient Patient { get; set; } = null!;

        public int MedicineId { get; set; }

        [ForeignKey(nameof(MedicineId))]
        public virtual Medicine Medicine { get; set; } = null!;
    }
}
