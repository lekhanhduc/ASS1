namespace FUNewsManagementSystem.Dtos.Response
{
    public class NewsArticleUpdateResponse
    {
        public int NewsArticleID { get; set; }
        public string NewsTitle { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public string NewsContent { get; set; } = string.Empty;
        public string NewsSource { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string ImageUrl { get; set; } = string.Empty;
        public string? NewsStatus { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
