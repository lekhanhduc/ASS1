using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagementSystem.Models
{
    public class NewsTag
    {
        public int NewsArticleID { get; set; }
        public int TagID { get; set; }

        // Navigation properties
        public NewsArticle NewsArticle { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }

}
