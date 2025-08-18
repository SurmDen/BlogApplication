using BlogApp.Data.Models;

namespace BlogApp.Interfaces
{
    public interface IBlogRepository
    {
        public Task CreateBlogAsync(Blog blog);

        public Task MakeFullTranslationAsync(long id);

        public Task UpdateBlogsCategoryAsync(long blogId, long categoryId);

        public Task UpdateBlogAsync(Blog blog);

        public Task UpdateSpecificBlogAsync(Blog blog);

        public Task DeleteBlogAsync(string blogAlias);

        public Task<Blog> GetBlogByIdAsync(long id);

        public Task<Blog> GetBlogByAliasAndLanguageAsync(string alias, string langCode);

        public Task<List<Blog>> GetBlogsOnSpecificLanguageAsync(string langCode);

        public Task<List<Blog>> GetBlogsAsync();

        public Task<List<Language>> GetAllLangsAsync();

        public Task<Title> GetTitleByLangCodeAsync(string lang);

        public Task<List<Tag>> GetSimplifiedTags();

        public Task<Blog> GetBlogAsNoTrackngAsync(long blogId);

        public Task CreateTagsAsync(List<Tag> tags);

        public Task<Tag> GetTagByNameAndLangAsync(string tagName, string lang);

        public Task AddVideoToBlogsAsync(string blogAlias, string videoName, string videoPath);

        public Task<List<Blog>> GetSimilarBlogsByTagsAsync(List<Tag> tags, string lang);

        public Task UpdateTagsAsync(string tagsString, string alias);

        public Task RemoveVideoAsync(string alias);
    }
}
