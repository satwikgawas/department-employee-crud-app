using EmployeeDepartmentCRUDApp.Enums;
using EmployeeDepartmentCRUDApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Employee")]
        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Please select employee")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Check In Time is required")]
        [Display(Name = "Check In Time")]
        [DataType(DataType.Time)]
        public DateTime CheckInTime { get; set; }
        [Required(ErrorMessage = "Check Out Time is required")]
        [Display(Name = "Check Out Time")]
        [DataType(DataType.Time)]
        public DateTime CheckOutTime { get; set; }
        [Required(ErrorMessage = "Please select status")]
        [Display(Name = "Status")]
        public Status Status { get; set; }
    }
}