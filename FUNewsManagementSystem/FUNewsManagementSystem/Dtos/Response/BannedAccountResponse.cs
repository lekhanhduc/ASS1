namespace FUNewsManagementSystem.Dtos.Response
{
    public class BannedAccountResponse
    {
        public int UserId { get; set; }
        public string? Reason { get; set; }
        public DateTime BannedAt { get; set; }
    }
}
