using BlogApp.Data.Helpers;
using BlogApp.Data.Models;

namespace BlogApp.Interfaces
{
    public interface IBotUserRepository
    {
        public Task<List<BotUserShortData>> GetAllBotUsersAsync();

        public Task<BotUserShortData> GetBotUserByTelegramIdAsync(long telegramId);

        public Task<BotUserShortData> GetBotUserByIdAsync(long id);

        public Task<BotUserShortData> AddBotUserAsync(CreateBotUserRequest botUserRequest);
    }
}
