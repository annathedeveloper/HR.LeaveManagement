using Microsoft.AspNetCore.Identity;

namespace HR.LeaveManagement.Identity.Exceptions
{
    public class IdentityException : ApplicationException
    {
        public List<string> Errors { get; set; } = new List<string>();
        public IdentityException(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                Errors.Add(error.Description);
            }
        }

    }
}
