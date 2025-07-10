using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        [Required(ErrorMessage ="Please select user")]
        [Display(Name ="User")]
        public int UserId { get; set; }
        public User? User { get; set; }
        [ForeignKey("Role")]
        [Required(ErrorMessage = "Please select role")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
