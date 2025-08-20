using BlogApp.Data;
using BlogApp.Data.Models;
using BlogApp.Infrastructure;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("blog")]
    public class SitemapController : Controller
    {
        public SitemapController(IBlogRepository blogRepository, ICategoryRepository categoryRepository, BlogContext blogContext)
        {
            this.blogRepository = blogRepository;
            this.categoryRepository = categoryRepository;
            this.blogContext = blogContext;
        }

        private IBlogRepository blogRepository;
        private ICategoryRepository categoryRepository;
        private BlogContext blogContext;

        [HttpGet("sitemap.xml")]
        public async Task<IActionResult> GetSitemapAsync()
        {
            string baseUrl = "https://bot-market.com";

            List<string> nodes = new List<string>();

            List<Language> languages = await blogRepository.GetAllLangsAsync();

            foreach (var language in languages)
            {
                string blogMainRef = $"{baseUrl}/blog/main/{language.LangName}";

                nodes.Add(blogMainRef);
            }

            foreach (var language in languages)
            {
                string blogsPageRef = $"{baseUrl}/blog/all/{language.LangName}";

                nodes.Add(blogsPageRef);
            }

            foreach (var language in languages)
            {
                List<Tag> tags = await blogContext.Tags.AsNoTracking().Include(t => t.Blogs).ToListAsync();

                foreach (Tag tag in tags)
                {
                    if (tag.Blogs.Count() > 0)
                    {
                        string tagPageRef = $"{baseUrl}/blog/all/tag/{tag.TagName}/{language.LangName}";

                        nodes.Add(tagPageRef);
                    }
                }
            }

            foreach (var language in languages)
            {
                List<Category> categories = await categoryRepository.
                    GetCategoriesOnSpecificLanguageAsync(language.LangName);

                foreach (var category in categories)
                {
                    if (category.Blogs != null)
                    {
                        if (category.Blogs.Count() > 0)
                        {
                            string categoryPageRef = $"{baseUrl}/blog/all/{category.Alias}/{language.LangName}";

                            nodes.Add(categoryPageRef);
                        }
                    }
                }
            }

            foreach (var language in languages)
            {
                List<Category> categories = await categoryRepository.
                    GetCategoriesOnSpecificLanguageAsync(language.LangName);

                foreach (Category category in categories)
                {
                    if (category.Blogs != null)
                    {
                        if (category.Blogs.Count() > 0)
                        {
                            foreach (Blog blog in category.Blogs.Where(b => b.IsArchived == false))
                            {
                                string blogPageRef = $"{baseUrl}/blog/{category.Alias}/{blog.Alias}/{language.LangName}";

                                nodes.Add(blogPageRef);
                            }
                        }
                    }
                }
            }

            SitemapGenerator sitemapGenerator = new SitemapGenerator();

            string xmlContent = sitemapGenerator.GenerateDocument(nodes);

            return Content(xmlContent, "text/xml", Encoding.UTF8);
        }

        [HttpGet("rss-feed.xml")]
        public async Task<IActionResult> GetRssChannel()
        {
            List<Category> categories = await categoryRepository.GetAllCategoriesAsync();

            RSSGenerator rSSGenerator = new RSSGenerator();

            string rssStringContent = rSSGenerator.GenerateRssChannelString(categories);

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(rssStringContent);

            await Console.Out.WriteLineAsync(xmlDoc.InnerXml);

            return Content(xmlDoc.InnerXml, "text/xml", Encoding.UTF8);
        }
    }
}
