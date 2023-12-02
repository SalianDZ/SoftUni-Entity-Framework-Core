using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmacyDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(14)]
        [MaxLength(14)]
        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [XmlAttribute("non-stop")]
        public string IsNonStop { get; set; } = null!;

        public ImportMedicineInPharmacyDto[] Medicines { get; set; } = null!;
    }
}
