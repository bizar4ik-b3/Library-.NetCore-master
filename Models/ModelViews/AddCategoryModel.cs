using System.ComponentModel.DataAnnotations;
namespace MvcMovie.Models
{
    public class AddCategoryModel
    {
        [Required(ErrorMessage = "Не вказано назви")]
        [StringLength(28, MinimumLength = 3, ErrorMessage = "Довжина назви категорії за велика!")]

        [Display(Name = "Назва категорії")]  
        public string Name { get; set; }
         
        [Display(Name = "Оберіть батківську категорію")]  
        public int categories { get; set; }

        [Display(Name = "Короткий опис категорії")]
        public string Desc1 { get; set; }
    }
}
