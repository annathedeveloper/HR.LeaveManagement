using System.ComponentModel.DataAnnotations;

namespace HR.LeaveManagement.MVC.Models
{
    public class RegisterVM
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?!.*\.).*", ErrorMessage = "Password must have at least one non-alpha character and at least one digit ('0'-'9')")]
        public string Password { get; set; }
    }
}
