using BlogApp.Data.Helpers;
using BlogApp.Data.Models;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("blog/api/comment")]
    public class CommentController : Controller
    {
        public CommentController(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        private ICommentRepository commentRepository;

        [HttpGet("get/all/user/{userId}")]
        public async Task<IActionResult> GetUserCommentsAsync(long userId)
        {
            try
            {
                List<Comment> comments = await commentRepository.GetUserCommentsAsync(userId);

                return Ok(comments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get/all/blog/{blogId}")]
        public async Task<IActionResult> GetBlogCommentsAsync(long blogId)
        {
            try
            {
                List<Comment> comments = await commentRepository.GetBlogCommentsAsync(blogId);

                return Ok(comments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("translate")]
        public async Task<IActionResult> TranslateCommentAsync(TranslateCommentModel model)
        {
            try
            {
                string translation = await commentRepository.TranslateCommentTextAsync(model.Text, model.LangCode);

                return Ok(translation);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("translate/withsaving")]
        public async Task<IActionResult> TranslateCommentAndSaveAsync(TranslateTargetModel model)
        {
            try
            {
                string translation = await commentRepository.TranslateCommentAndSaveAsync(model.TargetId, model.Text, model.LangCode);

                return Ok(translation);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("answer/translate/withsaving")]
        public async Task<IActionResult> TranslateCommentAnswerAndSaveAsync(TranslateTargetModel model)
        {
            try
            {
                string translation = await commentRepository.TranslateAnswerAndSaveAsync(model.TargetId, model.Text, model.LangCode);

                return Ok(translation);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCommentAsync(CreateCommentModel model)
        {
            try
            {
                await commentRepository.AddCommentAsync(model.Text, model.BlogAlias, model.UserId, model.LangCode);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCommentAsync(UpdateCommentMode model)
        {
            try
            {
                await commentRepository.UpdateCommentTextAsync(model.Text, model.CommentId);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete/{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync(long commentId)
        {
            try
            {
                await commentRepository.DeleteCommentAsync(commentId);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("answer/create")]
        public async Task<IActionResult> CreateAnswerAsync(CreateAnswerModel model)
        {
            try
            {
                await commentRepository.AddAnswerToCommentAsync(model.Text, model.CommentId, model.UserId, model.LangCode);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("answer/update")]
        public async Task<IActionResult> UpdateAnswerAsync(UpdateAnswerModel model)
        {
            try
            {
                await commentRepository.UpdateAnswerToCommentAsync(model.Text, model.AnswerId);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("answer/delete/{answerId}")]
        public async Task<IActionResult> DeleteAnswerAsync(long answerId)
        {
            try
            {
                await commentRepository.RemoveAnswerFromCommentAsync(answerId);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
