using System.ComponentModel.DataAnnotations.Schema;

namespace Artillery.Data.Models
{
    public class CountryGun
    {
        public int CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; } = null!;

        public int GunId { get; set; }

        [ForeignKey(nameof(GunId))]
        public virtual Gun Gun { get; set; } = null!;
    }
}
