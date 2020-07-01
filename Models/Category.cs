using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "Назва категорії")]  
        [StringLength(28, MinimumLength = 3, ErrorMessage = "Довжина назви категорії за велика!")]
        public string Name { get; set; }

        [Display(Name = "Опис категорії")]  
        public string Desc1 { get; set; }
        public int parent_id { get; set; }
        public string Desc2 { get; set; }
    }
}

