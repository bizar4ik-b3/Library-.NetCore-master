using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Не указан Email")]
        public string RegisterEmail { get; set; }
         
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public int RegisterPassword { get; set; }
    }
}