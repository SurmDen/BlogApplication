using BlogApp.Data.Models;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BlogApp.Infrastructure;

namespace BlogApp.Controllers
{
    [Route("blog/api")]
    [ApiController]
    public class BlogController : Controller
    {
        public BlogController(IBlogRepository blogRepository, ILogger<BlogController> logger, IWebHostEnvironment environment)
        {
            this.blogRepository = blogRepository;
            this.logger = logger;
            this.environment = environment;
        }

        private IBlogRepository blogRepository;

        private IWebHostEnvironment environment;

        private ILogger<BlogController> logger;

        [HttpPost("create")]
        public async Task<IActionResult> CreateBlogAsync([FromBody] Blog blog)
        {

            try
            {
                if (!string.IsNullOrEmpty(blog.ImageBase64String))
                {

                    string imageName = $"image_{Guid.NewGuid()}";

                    string imageJpgName = imageName + ".jpg";

                    string imageWebpName = imageName + "_.webp";

                    string imagePath = $"{environment.WebRootPath}/blog_images/{imageJpgName}";

                    string imageWebpPath = $"{environment.WebRootPath}/blog_images/{imageWebpName}";

                    System.IO.File.WriteAllBytes(imagePath, Convert.FromBase64String(blog.ImageBase64String));

                    ImageConvertor imageConvertor = new ImageConvertor();

                    imageConvertor.ConvertToWebpFormat(imagePath, imageWebpPath);

                    blog.Image = $"/blog_images/{imageWebpName}";
                }

                foreach (var s in blog.Sections)
                {
                    foreach (var sub in s.Subsections)
                    {

                        foreach (var p in sub.Paragraphs)
                        {

                            if (!string.IsNullOrEmpty(p.ImageBase64String))
                            {
                                string imageName2 = $"image_{Guid.NewGuid()}";

                                string imageJpgName2 = imageName2 + ".jpg";

                                string imageWebpName2 = imageName2 + "_.webp";

                                string imagePath2 = $"{environment.WebRootPath}/blog_images/{imageJpgName2}";

                                string imageWebpPath2 = $"{environment.WebRootPath}/blog_images/{imageWebpName2}";

                                System.IO.File.WriteAllBytes(imagePath2, Convert.FromBase64String(p.ImageBase64String));

                                ImageConvertor imageConvertor2 = new ImageConvertor();

                                imageConvertor2.ConvertToWebpFormat(imagePath2, imageWebpPath2);

                                p.Image = $"/blog_images/{imageWebpName2}";
                            }
                        }
                    }
                }

                await blogRepository.CreateBlogAsync(blog);

                logger.LogInformation("blog with title: {@title} added", blog.Title);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while try to add blog, message: {@mess}", e.Message);

                await Console.Out.WriteLineAsync("controller error (create blog)");

                return BadRequest();
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBlogAsync([FromBody] Blog blog)
        {
            await Console.Out.WriteLineAsync("update 0 !!!!!!!!!!!!!!!!!");

            try
            {
                if (!string.IsNullOrEmpty(blog.ImageBase64String))
                {
                    string imageName = $"image_{Guid.NewGuid()}";

                    string imageJpgName = imageName + ".jpg";

                    string imageWebpName = imageName + "_.webp";

                    string imagePath = $"{environment.WebRootPath}/blog_images/{imageJpgName}";

                    string imageWebpPath = $"{environment.WebRootPath}/blog_images/{imageWebpName}";

                    System.IO.File.WriteAllBytes(imagePath, Convert.FromBase64String(blog.ImageBase64String));

                    ImageConvertor imageConvertor = new ImageConvertor();

                    imageConvertor.ConvertToWebpFormat(imagePath, imageWebpPath);

                    blog.Image = $"/blog_images/{imageWebpName}";
                }

                foreach (var s in blog.Sections)
                {
                    foreach (var sub in s.Subsections)
                    {

                        foreach (var p in sub.Paragraphs)
                        {

                            if (!string.IsNullOrEmpty(p.ImageBase64String))
                            {
                                string imageName2 = $"image_{Guid.NewGuid()}";

                                string imageJpgName2 = imageName2 + ".jpg";

                                string imageWebpName2 = imageName2 + "_.webp";

                                string imagePath2 = $"{environment.WebRootPath}/blog_images/{imageJpgName2}";

                                string imageWebpPath2 = $"{environment.WebRootPath}/blog_images/{imageWebpName2}";

                                System.IO.File.WriteAllBytes(imagePath2, Convert.FromBase64String(p.ImageBase64String));

                                ImageConvertor imageConvertor2 = new ImageConvertor();

                                imageConvertor2.ConvertToWebpFormat(imagePath2, imageWebpPath2);

                                p.Image = $"/blog_images/{imageWebpName2}";
                            }
                        }
                    }
                }

                await blogRepository.UpdateBlogAsync(blog);

                logger.LogInformation("blog with id: {@id} updated", blog.Id);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while try to update blog, message: {@mess}", e.Message);

                return BadRequest();
            }
        }

        [HttpPut("update/specific")]
        public async Task<IActionResult> UpdateSpecificBlogAsync([FromBody] Blog blog)
        {
            try
            {
                if (!string.IsNullOrEmpty(blog.ImageBase64String))
                {
                    string imageName = $"image_{Guid.NewGuid()}";

                    string imageJpgName = imageName + ".jpg";

                    string imageWebpName = imageName + "_.webp";

                    string imagePath = $"{environment.WebRootPath}/blog_images/{imageJpgName}";

                    string imageWebpPath = $"{environment.WebRootPath}/blog_images/{imageWebpName}";

                    System.IO.File.WriteAllBytes(imagePath, Convert.FromBase64String(blog.ImageBase64String));

                    ImageConvertor imageConvertor = new ImageConvertor();

                    imageConvertor.ConvertToWebpFormat(imagePath, imageWebpPath);

                    blog.Image = $"/blog_images/{imageWebpName}";
                }

                foreach (var s in blog.Sections)
                {
                    foreach (var sub in s.Subsections)
                    {

                        foreach (var p in sub.Paragraphs)
                        {

                            if (!string.IsNullOrEmpty(p.ImageBase64String))
                            {
                                string imageName2 = $"image_{Guid.NewGuid()}";

                                string imageJpgName2 = imageName2 + ".jpg";

                                string imageWebpName2 = imageName2 + "_.webp";

                                string imagePath2 = $"{environment.WebRootPath}/blog_images/{imageJpgName2}";

                                string imageWebpPath2 = $"{environment.WebRootPath}/blog_images/{imageWebpName2}";

                                System.IO.File.WriteAllBytes(imagePath2, Convert.FromBase64String(p.ImageBase64String));

                                ImageConvertor imageConvertor2 = new ImageConvertor();

                                imageConvertor2.ConvertToWebpFormat(imagePath2, imageWebpPath2);

                                p.Image = $"/blog_images/{imageWebpName2}";
                            }
                        }
                    }
                }

                await blogRepository.UpdateSpecificBlogAsync(blog);

                logger.LogInformation("blog with id: {@id} updated", blog.Id);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while try to update blog, message: {@mess}", e.Message);

                return BadRequest();
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBlogAsync([FromBody] string alias)
        {
            try
            {
                await blogRepository.DeleteBlogAsync(alias);

                logger.LogInformation("blogs with alias: {@alias} deleted", alias);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while try to delete blogs, message: {@mess}, alias: {@alias}", e.Message, alias);

                return BadRequest();
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetBlogByIdAsync(long id)
        {
            try
            {
                Blog blog = await blogRepository.GetBlogByIdAsync(id);

                return Ok(blog);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while try to get blog, message: {@mess}, id: {@id}", e.Message, id);

                return NotFound();
            }
        }

        [HttpGet("get/{alias}/{langCode}")]
        public async Task<IActionResult> GetBlogByAliasAndLanguageAsync(string alias, string langCode)
        {
            try
            {
                Blog blog = await blogRepository.GetBlogByAliasAndLanguageAsync(alias, langCode);

                return Ok(blog);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while try to get blog, " +
                    "message: {@mess}, alias: {@alias}, lang: {@lang}", e.Message, alias, langCode);

                return NotFound();
            }
        }

        [HttpGet("get/lang/{langCode}")]
        public async Task<IActionResult> GetBlogsOnSpecificLanguageAsync(string langCode)
        {
            try
            {
                List<Blog> blogs = await blogRepository.GetBlogsOnSpecificLanguageAsync(langCode);

                return Ok(blogs);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while try to get all blogs, " +
                    "message: {@mess}, lang: {@lang}", e.Message, langCode);

                return NotFound();
            }
        }
    }
}
