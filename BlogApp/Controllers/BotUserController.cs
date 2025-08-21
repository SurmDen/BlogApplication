using BlogApp.Data.Helpers;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("blog/api/botuser")]
    public class BotUserController : Controller
    {
        public BotUserController(IBotUserRepository botUserRepository)
        {
            this.botUserRepository = botUserRepository;
        }

        private IBotUserRepository botUserRepository;

        [HttpGet("v1/get/all")]
        public async Task<List<BotUserShortData>> GetAllBotUsersAsync()
        {
            try
            {
                return await botUserRepository.GetAllBotUsersAsync();
            }
            catch (InvalidOperationException)
            {
                return new List<BotUserShortData>();
            }
        }

        [HttpGet("v1/get/{id:long}")]
        public async Task<BotUserShortData> GetBotUserByIdAsync(long id)
        {
            try
            {
                return await botUserRepository.GetBotUserByIdAsync(id);
            }
            catch (InvalidOperationException)
            {
                return new BotUserShortData();
            }
        }

        [HttpGet("v1/get/telegram/{id:long}")]
        public async Task<BotUserShortData> GetBotUserByTelegramIdAsync(long id)
        {
            try
            {
                return await botUserRepository.GetBotUserByTelegramIdAsync(id);
            }
            catch (InvalidOperationException)
            {
                return new BotUserShortData();
            }
        }

        [HttpPost("v1/create")]
        public async Task<BotUserShortData> AddBotUserAsync([FromBody]CreateBotUserRequest createBotUserRequest)
        {
            return await botUserRepository.AddBotUserAsync(createBotUserRequest);
        }

        [HttpGet("v1/storage/set/{id:long}")]
        public async Task SetBotUserByIdAsync(long id)
        {
            try
            {
                BotUserShortData botUser =  await botUserRepository.GetBotUserByIdAsync(id);

                HttpContext.Session.SetString("current_bot_user", JsonConvert.SerializeObject(botUser));
            }
            catch (InvalidOperationException)
            {

            }
        }

        [HttpGet("v1/storage/remove")]
        public void RemoveBotUserFromStorageAsync()
        {
            try
            {
                HttpContext.Session.Remove("current_bot_user");
            }
            catch (InvalidOperationException)
            {

            }
        }
    }
}
