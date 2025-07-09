using EmployeeDepartmentCRUDApp.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class Leave
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        [Required(ErrorMessage = "Please select employee")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Please select leave type")]
        [Display(Name = "Leave Type")]
        public LeaveType LeaveType { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [Display(Name = "Reason")]
        public string Reason { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Please select status")]
        public Status Status { get; set; }
    }
}
