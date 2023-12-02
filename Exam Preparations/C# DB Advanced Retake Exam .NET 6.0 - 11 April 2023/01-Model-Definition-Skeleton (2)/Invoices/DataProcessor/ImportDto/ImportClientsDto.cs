using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ImportClientsDto
    {
        [Required]
        [MinLength(10)]
        [MaxLength(25)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(10)]
        [MaxLength(15)]
        [XmlElement("NumberVat")]
        public string NumberVat { get; set; } = null!;

        [XmlArray("Addresses")]
        public ImportAddressInClientDto[] Addresses { get; set; } = null!;
    }
}
