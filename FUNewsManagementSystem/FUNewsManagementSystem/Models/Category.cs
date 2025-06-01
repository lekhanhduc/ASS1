using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagementSystem.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string? Description { get; set; }

        public int? ParentCategoryID { get; set; }

        public bool IsActive { get; set; }

        public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    }
}
