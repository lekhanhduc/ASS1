namespace FUNewsManagementSystem.Dtos.Request
{
    public class CategoryCreationRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
