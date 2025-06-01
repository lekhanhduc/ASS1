namespace FUNewsManagementSystem.Dtos.Response
{
    public class CategoryDetailResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? ParentCategoryId { get; set; }
        public CategoryDetailResponse(int categoryId, string categoryName, string? description, bool isActive, int? parentCategoryId)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            Description = description;
            IsActive = isActive;
            ParentCategoryId = parentCategoryId;
        }

    }
}
