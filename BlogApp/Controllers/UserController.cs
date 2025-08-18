using BlogApp.Data.Helpers;
using BlogApp.Data.Models;
using BlogApp.Infrastructure;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [Route("blog/api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserRepository userRepository;

        private ILogger<UserController> logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] SigningData signingData)
        {
            try
            {
                User user = await userRepository.GetUserByNameAndPasswordAsync(signingData);

                SigningModel signingModel = new SigningModel()
                {
                    UserEmail = user.Email,
                    UserName = user.UserName,
                    UserRole = user.Role.RoleName
                };

                SigningManager signingManager = new SigningManager();

                await signingManager.LoginAsync(signingModel, HttpContext);

                logger.LogInformation("user with email: {@email} logged in", signingModel.UserEmail);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to log in, message: {@message}", e.Message);

                return BadRequest();
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            try
            {
                SigningManager signingManager = new SigningManager();

                await signingManager.LogoutAsync(HttpContext);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to log out, message: {@message}", e.Message);

                return BadRequest();
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserModel createUserModel)
        {
            try
            {
                await userRepository.CreateUserAsync(createUserModel);

                logger.LogInformation("user account with email: {@email} created", createUserModel.Email);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to create user, " +
                    "message: {@message}, email: {@email}", e.Message, createUserModel.Email);

                return BadRequest();
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] User user)
        {
            try
            {
                await userRepository.UpdateUserAsync(user);

                logger.LogInformation("user account with id {@id} updated", user.Id);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to update user, " +
                    "message: {@message}, id: {@id}", e.Message, user.Id);

                return BadRequest();
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] long userId)
        {
            try
            {
                await userRepository.DeleteUserAsync(userId);

                logger.LogInformation("user account with id: {@id} deleted", userId);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to delete user, " +
                    "message: {@message}, id: {@id}", e.Message, userId);

                return BadRequest();
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetUserByIdAsync(long id)
        {
            try
            {
                User user = await userRepository.GetUserByIdAsync(id);

                return Ok(user);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to get user, " +
                    "message: {@message}, id: {@id}", e.Message, id);

                return NotFound();
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetUsersAsync()
        {
            try
            {
                List<User> users = await userRepository.GetUsersAsync();

                return Ok(users);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured while trying to get all users, message: {@message}", e.Message);

                return NotFound();
            }
        }
    }
}
