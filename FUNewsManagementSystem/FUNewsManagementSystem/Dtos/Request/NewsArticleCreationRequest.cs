namespace FUNewsManagementSystem.Dtos.Request
{
    public class NewsArticleCreationRequest
    {
        public string NewsTitle { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public string NewsContent { get; set; } = string.Empty;
        public string NewsSource { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public IFormFile? ImageFile { get; set; }
    }

}
