using BlogApp.Data.Models;

namespace BlogApp.Interfaces
{
    public interface ICategoryRepository
    {
        public Task CreateCategoryAsync(Category category);

        public Task UpdateCategoryAsync(Category category);

        public Task DeleteCategoryAsync(string alias);

        public Task<Category> GetCategoryByIdAsync(long id);

        public Task<Category> GetCategoryByAliasAndLanguageAsync(string alias, string langCode);

        public Task<List<Category>> GetCategoriesOnSpecificLanguageAsync(string langCode);

        public Task<List<Category>> GetSimpleCategoriesOnSpecificLanguageAsync(string langCode);

        public Task<List<Category>> GetAllCategoriesAsync();

        public bool IsCategoriesEmpty();
    }
}
