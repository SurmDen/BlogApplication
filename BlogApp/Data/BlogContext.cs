using BlogApp.Data.Helpers;
using BlogApp.Data.Models;
using BlogApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
            
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Subsection> Subsections { get; set; }

        public DbSet<Paragraph> Paragraphs { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<CommentTranslation> CommentTranslations { get; set; }

        public DbSet<AnswerTranslation> AnswerTranslations { get; set; }

        public DbSet<Title> Titles { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<CommentAnswer> CommentAnswers { get; set; }

        public DbSet<BotUser> BotUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new Role[]
            {
                new Role()
                {
                    Id = 1,
                    RoleName = "Admin"
                },
                new Role()
                {
                    Id = 2,
                    RoleName = "User"
                }
            });

            modelBuilder.Entity<User>().HasData(new User[]
            {
                new User()
                {
                    Id = 1,
                    UserName = "Alex_Purtov",
                    Email = "purtovalex@gmail.com",
                    Password = "123456",
                    RoleId = 1
                },
                new User()
                {
                    Id = 2,
                    UserName = "Surm_Den",
                    Email = "surmanidzedenis609@gmail.com",
                    Password = "Kursant55!",
                    RoleId = 1
                }
            });

            Language[] languages = new Language[]
            {
                new Language()
                {
                    Id= 1,
                    LangName = "ru",
                    FullLangName = "русский",
                    CountryCode = "ru"

                },
                new Language()
                {
                    Id= 2,
                    LangName = "en",
                    FullLangName = "english",
                    CountryCode = "gb"
                },
                new Language()
                {
                    Id= 3,
                    LangName = "fr",
                    FullLangName = "français",
                    CountryCode= "fr"
                },
                new Language()
                {
                    Id= 4,
                    LangName = "de",
                    FullLangName = "deutsch",
                    CountryCode = "de"
                },
                new Language()
                {
                    Id= 5,
                    LangName = "ka",
                    FullLangName = "ქართული",
                    CountryCode = "ge"
                },
                new Language()
                {
                    Id= 6,
                    LangName = "az",
                    FullLangName = "azərbaycan",
                    CountryCode = "az"
                },
                new Language()
                {
                    Id= 7,
                    LangName = "ar",
                    FullLangName = "العربية",
                    CountryCode = "ir"
                },
                new Language()
                {
                    Id= 8,
                    LangName = "he",
                    FullLangName = "עברית",
                    CountryCode = "il"
                },
                new Language()
                {
                    Id= 9,
                    LangName = "ko",
                    FullLangName = "한국어",
                    CountryCode = "kr"
                },
                new Language()
                {
                    Id= 10,
                    LangName = "it",
                    FullLangName = "italiano",
                    CountryCode = "it"
                },
                new Language()
                {
                    Id= 11,
                    LangName = "pl",
                    FullLangName = "polski",
                    CountryCode = "pl"
                },
                new Language()
                {
                    Id= 12,
                    LangName = "tr",
                    FullLangName = "türk",
                    CountryCode = "tr"
                },
                new Language()
                {
                    Id= 13,
                    LangName = "zh",
                    FullLangName = "中文",
                    CountryCode = "cn"
                },
                new Language()
                {
                    Id= 14,
                    LangName = "tg",
                    FullLangName = "тоҷикӣ",
                    CountryCode = "tj"
                },
                new Language()
                {
                    Id= 15,
                    LangName = "uk",
                    FullLangName = "український",
                    CountryCode = "ua"
                },
                new Language()
                {
                    Id= 16,
                    LangName = "ja",
                    FullLangName = "日本語",
                    CountryCode = "jp"
                },
                new Language()
                {
                    Id= 17,
                    LangName = "es",
                    FullLangName = "español",
                    CountryCode = "es"
                },
                new Language()
                {
                    Id= 18,
                    LangName = "hi",
                    FullLangName = "हिन्दी",
                    CountryCode = "in"
                },
                new Language()
                {
                    Id= 19,
                    LangName = "bn",
                    FullLangName = "বাংলা",
                    CountryCode = "bd"
                },
                new Language()
                {
                    Id= 20,
                    LangName = "pt",
                    FullLangName = "português",
                    CountryCode = "pt"
                },
                new Language()
                {
                    Id= 21,
                    LangName = "ur",
                    FullLangName = "اردو",
                    CountryCode = "pk"
                },
                new Language()
                {
                    Id= 22,
                    LangName = "id",
                    FullLangName = "indonesia",
                    CountryCode = "id"
                },
                new Language()
                {
                    Id= 23,
                    LangName = "uz",
                    FullLangName = "O'zbek tili",
                    CountryCode = "uz"
                },
                new Language()
                {
                    Id= 24,
                    LangName = "vi",
                    FullLangName = "Tiếng việt",
                    CountryCode = "vn"
                },
                new Language()
                {
                    Id= 25,
                    LangName = "hy",
                    FullLangName = "Հայկական",
                    CountryCode = "am"
                },
                new Language()
                {
                    Id= 26,
                    LangName = "kk",
                    FullLangName = "Қазақша",
                    CountryCode = "kz"
                },
            };

            modelBuilder.Entity<Language>().HasData(languages);
        }
    }
}

