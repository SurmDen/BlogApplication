using BlogApp.Data.Models;
using BlogApp.Data.Helpers;

namespace BlogApp.Interfaces
{
    public interface IBlogTranslator
    {
        public Task TranslateVideoNameAsync(string blogAlias, string vName, string vPath);

        public Task MakeFullBlogsTranslationAsync(Blog blog, List<Language> languages);

        public Task TranslateNewBlogAsync(Blog blog);

        public Task UpdateBlogsTranslationAsync(Blog updatedBlog);

        public Task UpdateSpecificBlogsTranslationAsync(Blog updatedBlog, Blog updatedBlogFromDb);

        public Task TranslateNewCategoryAsync(Category category);

        public Task UpdateCategoriesTranslationAsync(Category updatedCategory);

        public Task<Translation> TranslateAsync(string text, string langCode);
    }
}
