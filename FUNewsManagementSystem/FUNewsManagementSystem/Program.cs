using FUNewsManagementSystem.Configuration;
using FUNewsManagementSystem.Middlewares;
using FUNewsManagementSystem.Models;
using FUNewsManagementSystem.Repositories;
using FUNewsManagementSystem.Services;
using FUNewsManagementSystem.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FUNewsManagementSystem.Dtos.Response;
using Microsoft.AspNetCore.Identity;
using FUNewsManagementSystem.Data;

namespace FUNewsManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<NewsArticle>("NewsArticles");
            odataBuilder.EntitySet<Category>("Categories");
            odataBuilder.EntitySet<Tag>("Tags");
            odataBuilder.EntitySet<SystemAccount>("SystemAccounts");

            // Configure the NewsArticleDetailResponse entity
            var newsArticleEntity = odataBuilder.EntityType<NewsArticleDetailResponse>();
            newsArticleEntity.HasKey(x => x.NewsArticleID);

            builder.Services.AddControllers()
                 .AddOData(opt => opt.AddRouteComponents("odata", odataBuilder.GetEdmModel())
                                     .Filter()
                                     .OrderBy()
                                     .Count()
                                     .Select()
                                     .Expand()
                                     .SkipToken()
                                     .SetMaxTop(100))

                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                 });

            builder.Services.AddHttpClient<GoogleAuthClient>(client =>
            {
                client.BaseAddress = new Uri("https://oauth2.googleapis.com/");
            });

            builder.Services.AddHttpClient<GoogleUserInfoClient>(client =>
            {
                client.BaseAddress = new Uri("https://www.googleapis.com/");
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCustomJwtAuthentication(builder.Configuration); // JWT
            CorsConfiguration.ConfigureServices(builder.Services); // CORS
            builder.Services.AddScoped<PasswordHasher<SystemAccount>>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<SystemAccountRepository>();
            builder.Services.AddScoped<RoleRepository>();
            builder.Services.AddScoped<NewsArticleRepository>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<TagRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddSingleton<CloudinaryService>();
            builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IMailService, MailService>();

            var app = builder.Build();

            // Seed dữ liệu
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<SystemAccount>>();
                DatabaseSeeder.SeedAsync(dbContext, passwordHasher).GetAwaiter().GetResult();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseMiddleware<GlobalExceptionHandling>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}