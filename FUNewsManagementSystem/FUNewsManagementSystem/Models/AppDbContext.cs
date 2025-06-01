using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Models
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SystemAccount> SystemAccounts { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<NewsTag> NewsTags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SystemAccount>()
                .Property(a => a.userStatus)
                .HasConversion<string>() // chuyển enum thành string
                .HasMaxLength(20);

            // Cấu hình khóa chính ghép cho NewsTag
            modelBuilder.Entity<NewsTag>()
                .HasKey(nt => new { nt.NewsArticleID, nt.TagID });

            // Cấu hình quan hệ giữa NewsTag và NewsArticle
            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.NewsArticle)
                .WithMany(n => n.NewsTags)
                .HasForeignKey(nt => nt.NewsArticleID);

            // Cấu hình quan hệ giữa NewsTag và Tag
            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.Tag)
                .WithMany(t => t.NewsTags)
                .HasForeignKey(nt => nt.TagID);

        }
    }

}
