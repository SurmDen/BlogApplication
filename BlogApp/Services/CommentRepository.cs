using BlogApp.Data;
using BlogApp.Data.Helpers;
using BlogApp.Data.Models;
using BlogApp.Infrastructure;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services
{
    public class CommentRepository : ICommentRepository
    {
        public CommentRepository(BlogContext context, IBlogTranslator blogTranslator)
        {
            this.context = context;
            this.blogTranslator = blogTranslator;
        }

        private readonly BlogContext context;
        private readonly IBlogTranslator blogTranslator;

        public async Task AddCommentAsync(string text, string blogAlias, long userId, string langCode)
        {
            Comment comment = new Comment()
            {
                TextContent = text,
                BotUserId = userId
            };

            Translation translation = await blogTranslator.TranslateAsync(text, langCode);

            string detectedLangCode = translation.detectedLanguageCode;

            Language? language = await context.Languages.FirstOrDefaultAsync(l => l.LangName == detectedLangCode);

            if (language == null) 
            {
                language = await context.Languages.FirstOrDefaultAsync(l => l.LangName == langCode);
            }

            comment.LanguageId = language.Id;

            await context.Comments.AddAsync(comment);

            List<Blog> blogs = await context.Blogs.
                Include(b => b.Comments).
                Where(b => b.Alias == blogAlias).
                ToListAsync();

            foreach (var blog in blogs)
            {
                blog.Comments.Add(comment);
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateCommentTextAsync(string text, long commentId)
        {
            Comment? comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment != null)
            {
                comment.TextContent = text;

                context.Comments.Update(comment);

                await context.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException($"comment with id: {commentId} is null");
            }
        }

        public async Task DeleteCommentAsync(long commentId)
        {
            Comment? comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment != null)
            {
                context.Comments.Remove(comment);

                await context.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException($"comment with id: {commentId} is null");
            }
        }

        public async Task AddAnswerToCommentAsync(string text, long commentId, long userId, string langCode)
        {
            Comment? comment = await context.Comments
                .Include(c => c.CommentAnswers)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment != null)
            {
                CommentAnswer commentAnswer = new CommentAnswer()
                {
                    TextContent = text,
                    BotUserId = userId,
                    CommentId = commentId
                };

                Translation translation = await blogTranslator.TranslateAsync(text, langCode);

                string detectedLangCode = translation.detectedLanguageCode;

                Language? language = await context.Languages.FirstOrDefaultAsync(l => l.LangName == detectedLangCode);

                if (language == null)
                {
                    language = await context.Languages.FirstOrDefaultAsync(l => l.LangName == langCode);
                }

                commentAnswer.LanguageId = language.Id;

                context.CommentAnswers.Add(commentAnswer);

                await context.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException($"comment with id: {commentId} is null");
            }
        }

        public async Task UpdateAnswerToCommentAsync(string text, long answerId)
        {
            CommentAnswer? answer = await context.CommentAnswers.FirstOrDefaultAsync(c => c.Id == answerId);

            if (answer != null)
            {
                answer.TextContent = text;

                context.CommentAnswers.Update(answer);

                await context.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException($"comment with id: {answerId} is null");
            }
        }

        public async Task RemoveAnswerFromCommentAsync(long answerId)
        {
            CommentAnswer? answer = await context.CommentAnswers.FirstOrDefaultAsync(c => c.Id == answerId);

            if (answer != null)
            {
                context.CommentAnswers.Remove(answer);

                await context.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException($"comment with id: {answerId} is null");
            }
        }

        public async Task<List<Comment>> GetBlogCommentsAsync(long blogId)
        {
            Blog? blog = await context.Blogs.
                Include(b => b.Comments).
                ThenInclude(c => c.CommentAnswers).
                FirstOrDefaultAsync(b => b.Id == blogId);

            if (blog != null)
            {
                foreach (Comment comment in blog.Comments)
                {
                    await context.Entry(comment).Reference(c => c.Language).LoadAsync();
                    await context.Entry(comment).Reference(c => c.BotUser).LoadAsync();
                    await context.Entry(comment).Collection(c => c.CommentTranslations).LoadAsync();

                    if (comment.CommentAnswers!= null)
                    {
                        if (comment.CommentAnswers.Count > 0)
                        {
                            foreach (var answer in comment.CommentAnswers)
                            {
                                await context.Entry(answer).Collection(an => an.AnswerTranslations).LoadAsync();
                                await context.Entry(answer).Reference(a => a.Language).LoadAsync();
                            }
                        }
                    }
                }

                return blog.Comments;
            }
            else
            {
                throw new NullReferenceException($"blog with id: {blogId} is null");
            }
        }

        public async Task<List<Comment>> GetUserCommentsAsync(long userId)
        {
            BotUser? user = await context.BotUsers.
                AsNoTracking().
                Include(b => b.Comments).
                ThenInclude(c => c.CommentAnswers).
                FirstOrDefaultAsync(b => b.Id == userId);

            if (user != null)
            {
                foreach (Comment comment in user.Comments)
                {
                    await context.Entry(comment).Reference(c => c.Language).LoadAsync();
                }

                return user.Comments;
            }
            else
            {
                throw new NullReferenceException($"user with id: {userId} is null");
            }
        }

        public async Task<string> TranslateCommentTextAsync(string text, string langCode)
        {

            try
            {
                Translation translation = await blogTranslator.TranslateAsync(text, langCode);

                return translation.text;
            }
            catch (Exception e)
            {
                throw new Exception($"translate error, message: {e.Message}");
            }
        }

        public async Task<string> TranslateCommentAndSaveAsync(long commentId, string text, string langCode)
        {
            Comment? comment = await context.Comments.Include(c => c.CommentTranslations).FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment != null)
            {
                CommentTranslation? commentTranslation = comment.CommentTranslations.FirstOrDefault(c => c.LangCode == langCode);

                if (commentTranslation != null)
                {
                    return commentTranslation.Text;
                }
                else
                {
                    var translation = await blogTranslator.TranslateAsync(text, langCode);

                    commentTranslation = new CommentTranslation()
                    {
                        Text = translation.text,
                        LangCode = langCode
                    };

                    comment.CommentTranslations.Add(commentTranslation);

                    await context.SaveChangesAsync();

                    return translation.text;
                }
            }
            else
            {
                throw new NullReferenceException("comment is null");
            }
        }

        public async Task<string> TranslateAnswerAndSaveAsync(long answerId, string text, string langCode)
        {
            CommentAnswer? commentAnswer = await context.CommentAnswers.Include(c => c.AnswerTranslations).FirstOrDefaultAsync(c => c.Id == answerId);

            if (commentAnswer != null)
            {
                AnswerTranslation? answerTranslation = commentAnswer.AnswerTranslations.FirstOrDefault(c => c.LangCode == langCode);

                if (answerTranslation != null)
                {
                    return answerTranslation.Text;
                }
                else
                {

                    var translation = await blogTranslator.TranslateAsync(text, langCode);

                    answerTranslation = new AnswerTranslation()
                    {
                        Text = translation.text,
                        LangCode = langCode
                    };

                    commentAnswer.AnswerTranslations.Add(answerTranslation);

                    await context.SaveChangesAsync();

                    return translation.text;
                }
            }
            else
            {
                throw new NullReferenceException("comment is null");
            }
        }
    }
}
