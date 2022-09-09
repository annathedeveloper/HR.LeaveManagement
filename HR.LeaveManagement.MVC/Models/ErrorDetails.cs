namespace HR.LeaveManagement.MVC.Models
{
    public class ErrorDetails
    {
        public string Title { get; set; }
        public string Details { get; set; }
        public int Status { get; set; }

        public IReadOnlyDictionary<string, string[]> Errors = null;
    }
}
