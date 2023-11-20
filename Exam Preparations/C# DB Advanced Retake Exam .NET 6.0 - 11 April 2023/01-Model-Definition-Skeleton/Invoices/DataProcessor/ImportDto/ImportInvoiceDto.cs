using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoiceDto
    {
        //"Number": 1427940691,
        //"IssueDate": "2022-08-29T00:00:00",
        //"DueDate": "2022-10-28T00:00:00",
        //"Amount": 913.13,
        //"CurrencyType": 1,
        //"ClientId": 1


        [Required]
        [Range(1000000000, 1500000000)]
        public int Number { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [Range(0, 2)]
        public CurrencyType CurrencyType { get; set; }

        [Required]
        public int ClientId { get; set; }
    }
}
