using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class ImportTruckInDispatcherDto
    {
        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        [XmlElement("RegistrationNumber")]
        public string RegistrationNumber { get; set; } = null!;

        [Required]
        [MinLength(17)]
        [MaxLength(17)]
        [XmlElement("VinNumber")]
        public string VinNumber { get; set; } = null!;

        [Range(950, 1420)]
        [XmlElement("TankCapacity")]
        public int TankCapacity { get; set; }

        [Range(5000, 29000)]
        [XmlElement("CargoCapacity")]
        public int CargoCapacity { get; set; }

        [Required]
        [Range(0, 3)]
        [XmlElement("CategoryType")]
        public CategoryType CategoryType { get; set; }

        [Required]
        [Range(0, 4)]
        [XmlElement("MakeType")]
        public MakeType MakeType { get; set; }
    }
}
