using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoices.Data.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        public int Number { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public CurrencyType CurrencyType { get; set; }

        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public virtual Client Client { get; set; } = null!;
    }
}
