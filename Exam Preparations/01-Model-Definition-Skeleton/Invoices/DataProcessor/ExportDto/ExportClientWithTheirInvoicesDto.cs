using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Client")]
    public class ExportClientWithTheirInvoicesDto
    {
        [XmlAttribute("InvoicesCount")]
        public int InvoicesCount { get; set; }

        [XmlElement("ClientName")]
        [Required]
        [StringLength(25)]
        public string Name { get; set; } = null!;

        [XmlElement("VatNumber")]
        [Required]
        [StringLength(15)]
        public string NumberVat { get; set; } = null!;

        [XmlArray("Invoices")]
        public ExportInvoiceInClientDto[] Invoices { get; set; } = null!;
    }
}
