namespace FUNewsManagementSystem.Dtos.Request
{
    public class BannedAccountRequest
    {
        public int UserId { get; set; }

        public string Action { get; set; }
        public string? Reason { get; set; }
    }

}
