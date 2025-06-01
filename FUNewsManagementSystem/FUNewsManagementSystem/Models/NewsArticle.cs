using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagementSystem.Models
{
    public class NewsArticle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NewsArticleID { get; set; }
        public string NewsTitle { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string NewsContent { get; set; } = string.Empty;
        public string NewsSource { get; set; } = string.Empty;

        public string? ImageUrl { get; set; } 

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public string NewsStatus { get; set; } = "ACTIVE";

        public int CreatedByID { get; set; }
        [ForeignKey("CreatedByID")]
        public SystemAccount? SystemAccount { get; set; }

        public int? UpdatedByID { get; set; }
        public DateTime ModifiedDate { get; set; }

        public List<NewsTag> NewsTags { get; set; } = new List<NewsTag>();

    }
}
