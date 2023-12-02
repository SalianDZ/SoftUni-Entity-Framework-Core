using Invoices.Data.Models;
using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductsDto
    {
        public ImportProductsDto()
        {
            Clients = new HashSet<int>();
        }

        [Required]
        [MinLength(9)]
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        [Range(5.00, 1000.00)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 4)]
        public CategoryType CategoryType { get; set; }

        public ICollection<int> Clients { get; set; }
    }
}
