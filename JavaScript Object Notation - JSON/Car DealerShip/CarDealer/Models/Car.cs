namespace CarDealer.Models
{
    public class Car
    {
        public int Id { get; set; }

        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        public long TravelledDistance { get; set; }

        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();    

        public ICollection<PartCar> PartsCars { get; set; } = new HashSet<PartCar>();
    }
}
