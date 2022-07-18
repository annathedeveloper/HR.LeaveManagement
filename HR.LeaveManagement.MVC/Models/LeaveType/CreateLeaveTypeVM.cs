using System.ComponentModel.DataAnnotations;

namespace HR.LeaveManagement.MVC.Models.LeaveType
{
    public class CreateLeaveTypeVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name ="Default Number Of Days")]
        public int DefaultDays { get; set; }
    }
}
