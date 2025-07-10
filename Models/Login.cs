using System.ComponentModel.DataAnnotations;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class Login
    {
        [Required(ErrorMessage ="Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage ="Pasword is required")]
        public string Password { get; set; }
    }
}
