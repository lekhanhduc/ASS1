namespace FUNewsManagementSystem.Dtos.Request
{
    public class NewsArticleUpdateRequest
    {
        public int NewsArticleID { get; set; }
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public string? NewsContent { get; set; } 
        public string? NewsSource { get; set; }
        public int CategoryID { get; set; }
        public string? ImageUrl { get; set; }
        public string? NewsStatus { get; set; }
    }
}
