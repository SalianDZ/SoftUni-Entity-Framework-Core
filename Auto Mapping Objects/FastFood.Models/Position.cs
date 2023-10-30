namespace FastFood.Models
{
    using FastFoodCommon.EntityConfiguration;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Position
    {
        public Position()
        {
            Employees = new HashSet<Employee>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(ValidationConstants.PositionNameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }
    }
}