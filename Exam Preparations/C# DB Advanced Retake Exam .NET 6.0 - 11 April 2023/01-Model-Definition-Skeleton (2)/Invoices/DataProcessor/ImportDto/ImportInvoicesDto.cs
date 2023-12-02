using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoicesDto
    {
        [Range(1_000_000_000, 1_500_000_000)]
        public int Number { get; set; }

        [Required]
        public string IssueDate { get; set; } = null!;

        [Required]
        public string DueDate { get; set; } = null!;

        public decimal Amount { get; set; }

        [Required]
        [Range(0,2)]
        public CurrencyType CurrencyType { get; set; }

        public int ClientId { get; set; }
    }
}
