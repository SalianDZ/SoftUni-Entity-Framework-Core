using FastFoodCommon.EntityConfiguration;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Categories
{
    public class CreateCategoryInputModel
    {
        [MinLength(ViewModelValidation.CategoryNameMinLength)]
        [MaxLength(ViewModelValidation.CategoryNameMaxLength)]
        public string CategoryName { get; set; } = null!;
    }
}
