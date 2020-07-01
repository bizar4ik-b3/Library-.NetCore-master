using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcMovie.Models
{

    public class UserRole
    {
        [Key, Required]
        public int Id { get; set; }
 
        [Required,ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
 
        [Required, ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public Role Role { get; set; }

    }
}