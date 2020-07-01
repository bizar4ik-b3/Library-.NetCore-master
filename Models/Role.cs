using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class Role
    {
        
        public int id { get; set; }

        [Display(Name = "Роль")]
        public string name { get; set; }

        public string Desc1 { get; set; }
        
        public string Desc2 { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

    }
}