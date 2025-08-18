using BlogApp.Data.Models;

namespace BlogApp.Interfaces
{
    public interface ICommentRepository
    {
        public Task AddCommentAsync(string text, string blogAlias, long userId, string langCode);

        public Task UpdateCommentTextAsync(string text, long commentId);

        public Task DeleteCommentAsync(long commentId);

        public Task AddAnswerToCommentAsync(string text, long commentId, long userId, string langCode);

        public Task UpdateAnswerToCommentAsync(string text, long answerId);

        public Task RemoveAnswerFromCommentAsync(long answerId);

        public Task<List<Comment>> GetBlogCommentsAsync(long blogId);

        public Task<List<Comment>> GetUserCommentsAsync(long userId);

        public Task<string> TranslateCommentTextAsync(string text, string langCode);

        public Task<string> TranslateCommentAndSaveAsync(long commentId, string text, string langCode);

        public Task<string> TranslateAnswerAndSaveAsync(long answerId, string text, string langCode);

    }
}
