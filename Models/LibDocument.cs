
using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{


    public class LibDocument
    {
        public int id { get; set; }

        [Display(Name = "Назва документу")]
        public string Name { get; set; }


        public string Path { get; set; }

        public string DbName { get; set; }
        public string Desc2 { get; set; }

        [Display(Name = "Короткий опис документу")]
        public string Desc1 { get; set; }

        public string ContentType { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Опубліковано?")]
        public bool Published { get; set; }

        public bool DocChecked { get; set; }

        [Display(Name = "Доступ за посиланням")]
        public bool AccessLinkOnly { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? ExecutorId { get; set; }
        public User Executor { get; set; }
    }
}