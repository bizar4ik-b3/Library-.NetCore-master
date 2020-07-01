using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace MvcMovie.Models
{
    public class AddFileModelView
    {
        [Required(ErrorMessage = "Не вказано назви")]
        [Display(Name = "Назва документу")] 
        public string Name { get; set; }

        public string Path { get; set; }

        public string Desc2 { get; set; }

        [Display(Name = "Опублікувати?")] 
        public bool Published { get; set; }

        [Display(Name = "Доступ за посиланням")] 
        public bool AccessLinkOnly { get; set; }

        [Display(Name = "Короткий опис документа")] 
        public string Desc1 { get; set; }

        public int categories { get; set; }

        public IFormFile uploadedFile { get; set; }        
    }
}

