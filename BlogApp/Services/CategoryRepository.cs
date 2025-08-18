using BlogApp.Data;
using BlogApp.Data.Models;
using BlogApp.Infrastructure;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BlogContext _blogContext;

        private IBlogTranslator translator;

        public CategoryRepository(BlogContext blogContext, IBlogTranslator translator)
        {
            _blogContext = blogContext;

            this.translator = translator;
        }

        public async Task CreateCategoryAsync(Category category)
        {
            try
            {
                await translator.TranslateNewCategoryAsync(category);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка перевода", e);
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                await translator.UpdateCategoriesTranslationAsync(category);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка перевода", e);
            }
        }

        public async Task DeleteCategoryAsync(string alias)
        {
            IEnumerable<Category> categories = _blogContext.Categories.Where(c => c.Alias == alias);

            _blogContext.Categories.RemoveRange(categories);

            await _blogContext.SaveChangesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(long id)
        {
            Category category = await _blogContext.Categories.Include(c => c.Blogs).FirstAsync(c => c.Id == id);

            foreach (Blog blog in category.Blogs)
            {
                blog.Category = null;
            }

            return category;
        }

        public async Task<Category> GetCategoryByAliasAndLanguageAsync(string alias, string langCode)
        {
            Category category = await _blogContext.Categories.
                Include(c => c.Language).
                Include(c => c.Blogs).
                AsNoTracking().
                FirstAsync(c => c.Alias == alias && c.Language.LangName == langCode);

            category.Language.Categories = null;

            foreach (Blog blog in category.Blogs)
            {
                blog.Category = null;
            }

            return category;
        }

        public async Task<List<Category>> GetCategoriesOnSpecificLanguageAsync(string langCode)
        {
            IEnumerable<Category> categories = _blogContext.Categories.
                Include(c => c.Blogs).
                Include(c => c.Language).
                Where(c => c.Language.LangName == langCode);

            if (categories.Count() > 0)
            {
                foreach (Category category in categories)
                {
                    category.Language.Categories = null;

                    foreach (Blog blog in category.Blogs)
                    {
                        blog.Category = null;
                    }
                }
            }

            return categories.ToList();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            IEnumerable<Category> categories = _blogContext.Categories.
                Include(c => c.Language).
                Include(c => c.Blogs).
                ThenInclude(b => b.Sections).
                ThenInclude(s => s.Subsections).
                ThenInclude(p => p.Paragraphs);       

            if (categories.Count() > 0)
            {
                foreach (Category category in categories)
                {
                    category.Language.Categories = null;

                    foreach (Blog blog in category.Blogs)
                    {
                        blog.Category = null;
                    }
                }
            }

            return categories.ToList();
        }

        public async Task<List<Category>> GetSimpleCategoriesOnSpecificLanguageAsync(string langCode)
        {
            List<Category> categories = await _blogContext.Categories.
                Include(c => c.Language).
                Where(c => c.Language.LangName == langCode).
                AsNoTracking().
                ToListAsync();

            return categories;
        } 

        public bool IsCategoriesEmpty()
        {
            int count = _blogContext.Categories.Count();

            if (count > 0)
            {
                return false;
            }

            return true;
        }
    }
}

