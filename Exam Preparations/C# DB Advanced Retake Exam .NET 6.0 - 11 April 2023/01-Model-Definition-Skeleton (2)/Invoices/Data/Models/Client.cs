using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace Invoices.Data.Models
{
    public class Client
    {
        public Client()
        {
            Invoices = new HashSet<Invoice>();
            Addresses = new HashSet<Address>();
            ProductsClients = new HashSet<ProductClient>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string NumberVat { get; set; } = null!;

        public virtual ICollection<Invoice> Invoices { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        public virtual ICollection<ProductClient> ProductsClients { get; set; }
    }
}
