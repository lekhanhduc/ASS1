namespace FUNewsManagementSystem.Dtos.Response
{
    public class CategoryCreationResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public CategoryCreationResponse(int categoryId, string categoryName, string? description, bool isActive)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            Description = description;
            IsActive = isActive;
        }

    }
}
