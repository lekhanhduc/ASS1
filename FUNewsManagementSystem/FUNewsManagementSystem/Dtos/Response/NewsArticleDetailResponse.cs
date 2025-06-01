namespace FUNewsManagementSystem.Dtos.Response
{
    public class NewsArticleDetailResponse
    {
        public int NewsArticleID { get; set; }
        public string NewsTitle { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string NewsContent { get; set; } = string.Empty;
        public string NewsSource { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string? ImageUrl { get; set; }
        public string? NewsStatus { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
    }
}
