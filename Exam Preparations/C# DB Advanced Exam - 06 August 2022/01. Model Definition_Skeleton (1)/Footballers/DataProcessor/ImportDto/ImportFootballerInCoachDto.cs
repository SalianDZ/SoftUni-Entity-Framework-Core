using Footballers.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Footballer")]
    public class ImportFootballerInCoachDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("ContractStartDate")]
        public string ContractStartDate { get; set; } = null!;

        [Required]
        [XmlElement("ContractEndDate")]
        public string ContractEndDate { get; set; } = null!;

        [Required]
        [Range(0, 4)]
        [XmlElement("BestSkillType")]
        public BestSkillType BestSkillType { get; set; }

        [Required]
        [Range(0, 3)]
        [XmlElement("PositionType")]
        public PositionType PositionType { get; set; }
    }
}
