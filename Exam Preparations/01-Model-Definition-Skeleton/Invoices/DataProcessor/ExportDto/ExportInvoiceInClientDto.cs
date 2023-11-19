using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Invoice")]
    public class ExportInvoiceInClientDto
    {
        [Required]
        [XmlElement("InvoiceNumber")]
        public int Number { get; set; }

        [Required]
        [XmlElement("InvoiceAmount")]
        public decimal Amount { get; set; }

        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; } = null!;

        [Required]
        [XmlElement("Currency")]
        public CurrencyType CurrencyType { get; set; }
    }
}
