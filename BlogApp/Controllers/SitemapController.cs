using BlogApp.Data.Models;
using BlogApp.Infrastructure;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Text;
using System.Xml;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("blog")]
    public class SitemapController : Controller
    {
        public SitemapController(IBlogRepository blogRepository, ICategoryRepository categoryRepository)
        {
            this.blogRepository = blogRepository;
            this.categoryRepository = categoryRepository;
        }

        private IBlogRepository blogRepository;
        private ICategoryRepository categoryRepository;

        [HttpGet("sitemap.xml")]
        public async Task<IActionResult> GetSitemapAsync()
        {
            string baseUrl = "https://bot-market.com";

            List<string> nodes = new List<string>()
            {
                $"{baseUrl}/blog/main"
            };

            List<Language> languages = await blogRepository.GetAllLangsAsync();

            foreach (var language in languages)
            {
                string blogsPageRef = $"{baseUrl}/blogs/{language.LangName}";

                nodes.Add(blogsPageRef);

                List<Category> categories = await categoryRepository.
                    GetCategoriesOnSpecificLanguageAsync(language.LangName);

                if (categories.Count() > 0)
                {
                    foreach (Category category in categories)
                    {
                        string categoryPageRef = $"{baseUrl}/blogs/{category.Alias}/{language.LangName}";

                        nodes.Add(categoryPageRef);

                        if (category.Blogs.Count() > 0)
                        {
                            foreach (Blog blog in category.Blogs)
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
