using System.ComponentModel.DataAnnotations;

namespace Invoices.Data.Models
{
    public class Client
    {
        public Client()
        {
            Addresses = new HashSet<Address>();
            ProductsClients = new HashSet<ProductClient>();
            Invoices = new HashSet<Invoice>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string NumberVat { get; set; } = null!;

        public ICollection<Invoice> Invoices { get; set; }

        public ICollection<Address> Addresses { get; set; }

        public ICollection<ProductClient> ProductsClients { get; set; }
    }
}
