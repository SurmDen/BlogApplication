using BlogApp.Data.Models;
using BlogApp.Interfaces;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [Route("blog/api/category")]
    [ApiController]
    public class CategoryController : Controller
    {
        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
        {
            this.categoryRepository = categoryRepository;
            this.logger = logger;

        }

        private ICategoryRepository categoryRepository;
        private ILogger<CategoryController> logger;

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] Category category)
        {
            try
            {
                await categoryRepository.CreateCategoryAsync(category);

                logger.LogInformation("category with name: {@name} created", category.CategoryName);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to create category, message: {@message}", e.Message);

                return BadRequest();
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] Category category)
        {
            try
            {
                await categoryRepository.UpdateCategoryAsync(category);

                logger.LogInformation("categoty with id: {@id} updated", category.Id);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to update category," +
                    " message: {@message}, id: {@id}", e.Message, category.Id);

                return BadRequest();
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> RemoveCategoryAsync([FromBody] string alias)
        {
            try
            {
                await categoryRepository.DeleteCategoryAsync(alias);

                logger.LogInformation("categories with alias: {@alias} deleted", alias);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to remove categories, " +
                    "message: {@message}, alias: {@alias}", e.Message, alias);

                return BadRequest();
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(long id)
        {
            try
            {
                Category category = await categoryRepository.GetCategoryByIdAsync(id);

                return Ok(category);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to get category, " +
                    "message: {@message}, id: {@id}", e.Message, id);

                return NotFound();
            }
        }

        [HttpGet("get/{alias}/{langCode}")]
        public async Task<IActionResult> GetCategoriesByAliasAndLanguageAsync(string alias, string langCode)
        {
            try
            {
                Category category = await categoryRepository.GetCategoryByAliasAndLanguageAsync(alias, langCode);

                return Ok(category);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to get category," +
                    " message: {@message}, alias: {@alias}, lang: {@lang}", e.Message, alias, langCode) ;

                return NotFound();
            }
        }

        [HttpGet("get/lang/{langCode}")]
        public async Task<IActionResult> GetCategoriesOnSpecificLanguageAsync(string langCode)
        {
            try
            {
                List<Category> categories = await categoryRepository.GetCategoriesOnSpecificLanguageAsync(langCode);

                return Ok(categories);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to get all categories," +
                    " message: {@message}, lang: {@lang}", e.Message, langCode);

                return NotFound();
            }
        }
    }
}
