using BlogApp.Data;
using BlogApp.Data.Models;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;

namespace BlogApp.Services
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogContext _blogContext;

        private readonly IBlogTranslator translator;

        public BlogRepository(BlogContext blogContext, IBlogTranslator translator)
        {
            _blogContext = blogContext;
            this.translator = translator;
        }

        public async Task UpdateBlogsCategoryAsync(long blogId, long categoryId)
        {
            Blog blog = await _blogContext.Blogs.FirstAsync(b => b.Id == blogId);

            blog.CategoryId = categoryId;

            _blogContext.Blogs.Update(blog);

            await _blogContext.SaveChangesAsync();
        }

        public async Task<List<Language>> GetAllLangsAsync()
        {
            List<Language> langs =  await _blogContext.Languages.Include(l => l.Blogs).AsNoTracking().ToListAsync();

            foreach (var lang in langs)
            {
                foreach (var blog in lang.Blogs)
                {
                    blog.Language = null;
                }
            }

            return langs;
        }

        //WITH BLOG TRANSLATOR
        public async Task MakeFullTranslationAsync(long ruBlogId)
        {
            try
            {
                var blogOnRuLang = await _blogContext.Blogs
                    .Include(b => b.Tags)
                    .Include(b => b.User)
                    .Include(b => b.Sections)
                        .ThenInclude(s => s.Subsections)
                            .ThenInclude(s => s.Paragraphs)
                    .FirstOrDefaultAsync(b => b.Id == ruBlogId);

                if (blogOnRuLang == null)
                {
                    throw new ArgumentException($"Блог с ID {ruBlogId} не найден");
                }

                var existingLanguageIds = await _blogContext.Blogs
                    .Where(b => b.Alias == blogOnRuLang.Alias)
                    .Select(b => b.LanguageId)
                    .ToListAsync();

                var allLanguages = await _blogContext.Languages
                    .AsNoTracking()
                    .ToListAsync();

                var neededLanguages = allLanguages
                    .Where(l => !existingLanguageIds.Contains(l.Id))
                    .ToList();

                if (!neededLanguages.Any())
                {
                    return;
                }

                try
                {
                    await translator.MakeFullBlogsTranslationAsync(
                        blogOnRuLang,
                        neededLanguages);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в переводчике: {ex.Message}");
                    throw new Exception("Не удалось выполнить перевод", ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка в MakeFullTranslationAsync: {ex}");
                throw new Exception("Ошибка при выполнении полного перевода статьи", ex);
            }
        }

        //WITH BLOG TRANSLATOR
        public async Task CreateBlogAsync(Blog blog)
        {
            try
            {
                blog.DateOfPublish = DateTime.Now;

                if (!string.IsNullOrEmpty(blog.TagList))
                {
                    List<Tag> tags = new List<Tag>();

                    string[] tagsIdList = blog.TagList.Split(" ");

                    foreach (string tagString in tagsIdList)
                    {
                        if (tagString != " ")
                        {
                            bool isNum =  long.TryParse(tagString, out long tagId);

                            if (isNum)
                            {
                                Tag tag = await _blogContext.Tags.FirstAsync(t => t.Id == tagId);

                                if (tag != null)
                                {
                                    tags.Add(tag);
                                }
                            }
                        }
                    }

                    blog.Tags = tags;
                }

                await translator.TranslateNewBlogAsync(blog);

            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync("repository error (create blog)");

                throw new Exception("Ошибка возникла при создании и переводе статей", e);
            }
        }

        //WITH BLOG TRANSLATOR
        public async Task UpdateBlogAsync(Blog blog)
        {
            try
            {
                blog.DateOfUpdate = DateTime.Now;

                await translator.UpdateBlogsTranslationAsync(blog);

            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка возникла при обновлении статьи, детали: {e.Message}", e);
            }
        }

        //WITH BLOG TRANSLATOR
        public async Task UpdateSpecificBlogAsync(Blog blog)
        {
            try
            {
                blog.DateOfUpdate = DateTime.Now;
                
                Blog blogFromDb = await _blogContext.Blogs.
                    Include(b => b.Sections).
                    ThenInclude(s =>s.Subsections).
                    ThenInclude(sub => sub.Paragraphs).
                    FirstAsync(b => b.Id == blog.Id);

                await translator.UpdateSpecificBlogsTranslationAsync(blog, blogFromDb);

                _blogContext.Blogs.Update(blogFromDb);

                await _blogContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка возникла при обновлении статьи", e);
            }
        }

        public async Task DeleteBlogAsync(string blogAlias)
        {
            IEnumerable<Blog> blogs = _blogContext.Blogs.Where(b => b.Alias == blogAlias);

            //_blogContext.Blogs.RemoveRange(blogs);

            foreach (Blog blog in blogs)
            {
                blog.IsArchived = !blog.IsArchived;
            }

            await _blogContext.SaveChangesAsync();
        }

        public async Task DeleteBlogForeverAsync(string alias)
        {
            IEnumerable<Blog> blogs = _blogContext.Blogs.Where(b => b.Alias == alias);

            _blogContext.RemoveRange(blogs);

            await _blogContext.SaveChangesAsync();
        }

        public async Task UpdateTagsAsync(string tagsString, string alias)
        {
            List<Tag> tags = new List<Tag>();

            List<Blog> blogs = _blogContext.Blogs.Include(b => b.Tags).Where(b => b.Alias == alias).ToList();

            await Console.Out.WriteLineAsync($"!!!!!!!!!!!!!!!!!! blogs {blogs.Count}  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            if (!string.IsNullOrEmpty(tagsString))
            {
                string[] tagsIdList = tagsString.Split(" ");

                foreach (string tagString in tagsIdList)
                {
                    if (tagString != " ")
                    {
                        bool isNum = long.TryParse(tagString, out long tagId);

                        if (isNum)
                        {
                            Tag tag = await _blogContext.Tags.FirstAsync(t => t.Id == tagId);

                            if (tag != null)
                            {
                                tags.Add(tag);
                            }
                        }
                    }
                }
            }

            foreach (Blog blog in blogs)
            {
                if (tags.Count() > 0)
                {
                    blog.Tags = tags;
                }
                else
                {
                    blog.Tags = null;
                }
            }

            _blogContext.UpdateRange(blogs);

            await _blogContext.SaveChangesAsync();
        }

        public async Task<Blog> GetBlogByIdAsync(long id)
        {
            Blog blog = await _blogContext.Blogs.
                Include(b => b.User).
                Include(b =>b.Tags).
                Include(b => b.Sections).
                ThenInclude(s => s.Subsections).
                ThenInclude(s => s.Paragraphs).
                FirstAsync(b => b.Id == id);

            blog.User.Blogs = null;

            foreach (Tag tag in blog.Tags)
            {
                tag.Blogs = null;
            }

            foreach (Section section in blog.Sections)
            {
                section.Blog = null;

                foreach (Subsection subsection in section.Subsections)
                {
                    subsection.Section = null;

                    foreach (Paragraph paragraph in subsection.Paragraphs)
                    {
                        paragraph.Subsection = null;
                    }
                }
            }

            return blog;
        }

        public async Task<Blog> GetBlogByAliasAndLanguageAsync(string alias, string langCode)
        {
            try
            {
                Blog blog = await _blogContext.Blogs.
                Include(b => b.Category).
                Include(b => b.User).
                Include(b => b.Tags).
                Include(b => b.Sections).
                ThenInclude(s => s.Subsections).
                ThenInclude(s => s.Paragraphs).
                FirstAsync(b => b.Alias == alias && b.Language.LangName == langCode);

                blog.User.Blogs = null;

                blog.Category.Blogs = null;

                foreach (Tag tag in blog.Tags)
                {
                    tag.Blogs = null;
                }

                foreach (Section section in blog.Sections)
                {
                    section.Blog = null;

                    foreach (Subsection subsection in section.Subsections)
                    {
                        subsection.Section = null;

                        foreach (Paragraph paragraph in subsection.Paragraphs)
                        {
                            paragraph.Subsection = null;
                        }
                    }
                }

                return blog;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Blog>> GetBlogsOnSpecificLanguageAsync(string langCode)
        {
            IEnumerable<Blog> blogs = _blogContext.Blogs.
                Include(b => b.Category).
                Include(b => b.Tags).
                Include(b => b.Language).
                Where(b => b.Language.LangName == langCode).
                OrderByDescending(b => b.Id);

            foreach (Blog blog in blogs)
            {
                blog.Category.Blogs = null;

                blog.Language.Blogs = null;

                if (blog.Tags != null)
                {
                    if (blog.Tags.Count() > 0)
                    {
                        foreach (var tag in blog.Tags)
                        {
                            tag.Blogs = null;
                        }
                    }
                }
            }

            return blogs.ToList();
        }

        public async Task<List<Blog>> GetBlogsAsync()
        {
            IEnumerable<Blog> blogs = _blogContext.Blogs.
                Include(b => b.Category).
                Include(b => b.Language).
                OrderByDescending(b => b.Id).AsNoTracking();

            return blogs.ToList();
        }

        public async Task<Title> GetTitleByLangCodeAsync(string lang)
        {
            Language language = await _blogContext.Languages.Include(l => l.Title).
                FirstAsync(l => l.LangName == lang);

            return language.Title;
        }

        public async Task CreateTagsAsync(List<Tag> tags)
        {
            if (tags != null)
            {
                if (tags.Count > 0)
                {
                    List<Tag> tagsFromDB = _blogContext.Tags.AsNoTracking().ToList();

                    List<Tag> tagsForRemove = new List<Tag>();

                    foreach (var tg in tags)
                    {
                        if (tagsFromDB.Where(t => t.TagName.ToLower() == tg.TagName.ToLower()).Count() > 0)
                        {
                            tagsForRemove.Add(tg);
                        }
                    }

                    if (tagsForRemove.Count > 0)
                    {
                        foreach (var tg_rm in tagsForRemove)
                        {
                            tags.Remove(tg_rm);
                        }
                    }

                    _blogContext.Tags.AddRange(tags);

                    await _blogContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<Tag>> GetSimplifiedTags()
        {
            List<Tag> tags =  _blogContext.Tags.Include(t => t.Blogs).ToList();

            foreach (Tag tag in tags)
            {
                foreach (Blog blog in tag.Blogs)
                {
                    blog.Tags = null;
                }
            }

            return tags;
        }

        public async Task<Blog> GetBlogAsNoTrackngAsync(long blogId)
        {
            Blog blog = await _blogContext.Blogs.Include(b => b.Tags).AsNoTracking().FirstAsync(b => b.Id == blogId);

            foreach (Tag tag in blog.Tags)
            {
                tag.Blogs = null;
            }

            return blog;
        }

        public async Task<Tag> GetTagByNameAndLangAsync(string tagName, string lang)
        {
            try
            {
                Language language = _blogContext.Languages.First(l => l.LangName == lang);

                Tag tag = await _blogContext.Tags.
                Include(t => t.Blogs.Where(b => b.LanguageId == language.Id)).
                ThenInclude(b => b.Category).
                AsNoTracking().
                FirstAsync(t => t.TagName == tagName);

                foreach (Blog blog in tag.Blogs)
                {
                    blog.Tags = null;
                    blog.Category.Blogs = null;
                }

                return tag;
            }
            catch (Exception e)
            {
                throw new Exception("Tag not exists or not contained any Blogs");
            }
        }

        //WITH BLOG TRANSLATOR
        public async Task AddVideoToBlogsAsync(string blogAlias, string vName, string vPath)
        {
            await translator.TranslateVideoNameAsync(blogAlias, vName, vPath);
        }

        public async Task<List<Blog>> GetSimilarBlogsByTagsAsync(List<Tag> tags, string lang)
        {
            try
            {
                List<Blog> blogs = _blogContext.Blogs.
                Include(b => b.Category).
                Include(b => b.Tags).
                Include(b => b.Language).
                AsNoTracking().ToList();

                foreach (Tag tag in tags)
                {
                    blogs = blogs.Where(b => b.Tags.Where(t => t.Id == tag.Id).Count() > 0).ToList();
                }

                blogs = blogs.Where(b => b.Language.LangName == lang).ToList();

                return blogs;
            }
            catch (Exception e)
            {
                throw new Exception("there are no similar blogs", e);
            }
        }

        public async Task RemoveVideoAsync(string alias)
        {
            IEnumerable<Blog> blogs = _blogContext.Blogs.Where(b => b.Alias == alias);

            foreach (Blog blog in blogs)
            {
                blog.VideoName = "";
                blog.VideoPath = "";
            }

            _blogContext.Blogs.UpdateRange(blogs);

            await _blogContext.SaveChangesAsync();
        }
    }
}
