

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class ModuleAssign
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public bool IsOrganization {get;set;}
        public bool IsConfiguration { get;set;}
        public bool IsDepartment { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsUserManagement { get; set; }
        public bool IsUser { get; set; }
        public bool IsRole { get; set; }
        public bool IsUserRole { get; set; }
        public bool IsHR { get; set; }
        public bool IsPayroll { get; set; }
        public bool IsAttendance { get; set; }

        public bool IsLeave { get; set; }
    }
}
