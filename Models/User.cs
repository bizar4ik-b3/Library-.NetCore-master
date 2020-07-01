using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MvcMovie.Models
{
    public class User
    {
        public int id { get; set; }
        
        [Display(Name = "Імя")]
        [Required(ErrorMessage = "Не вказано ім'я")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string name { get; set; }

        [Display(Name = "Прізвище")]
        [Required(ErrorMessage = "Не вказано прізвище")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string surName { get; set; }

        [Display(Name = "По-батькові")]
        [Required(ErrorMessage = "Не вказано по-батькові")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string middleName { get; set; }

        [Display(Name = "Логін")]        
        public string login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не вказан пароль")]    

        public int password { get; set; }

         [Display(Name = "Виберіть роль")]  

         public int Rolesid { get; set; }

         [Display(Name = "Додаткові данні")]  

         public string Desc { get; set; }

          public Role Roles { get; set; }

         public ICollection<LibDocument> Documents { get; set; }

          public ICollection<UserRole> UserRoles { get; set; }
          
         
         
    }
    
}