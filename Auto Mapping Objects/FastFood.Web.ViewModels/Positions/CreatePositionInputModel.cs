using FastFoodCommon.EntityConfiguration;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Positions
{
    public class CreatePositionInputModel
    {
        //[MinLength(ViewModelValidation.PositionNameMinLength)]
        //[MaxLength(ViewModelValidation.PositionNameMaxLength)]
        [StringLength(ViewModelValidation.PositionNameMaxLength, MinimumLength = ViewModelValidation.PositionNameMinLength)]
        public string PositionName { get; set; } = null!;
    }
}