using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class LoginModel
    {

        [Required(ErrorMessage = "Не вказан Логін")]
        public string login { get; set; }
         
        [Required(ErrorMessage = "Не вказан пароль")]
        [DataType(DataType.Password)]

        public int LoginPassword { get; set; } //Change Password to string!!!!!
        

    }
}