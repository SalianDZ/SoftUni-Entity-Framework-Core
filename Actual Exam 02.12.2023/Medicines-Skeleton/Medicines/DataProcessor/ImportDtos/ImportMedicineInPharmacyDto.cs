using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Medicine")]
    public class ImportMedicineInPharmacyDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Range(0.01, 1000.00)]
        [XmlElement("Price")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 4)]
        [XmlAttribute("category")]
        public int Category { get; set; }

        [Required]
        [XmlElement("ProductionDate")]
        public string ProductionDate { get; set; } = null!;

        [Required]
        [XmlElement("ExpiryDate")]
        public string ExpiryDate { get; set; } = null!;

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        [XmlElement("Producer")]
        public string Producer { get; set; } = null!;
    }
}
