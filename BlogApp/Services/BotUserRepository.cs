using BlogApp.Data;
using BlogApp.Data.Helpers;
using BlogApp.Data.Models;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services
{
    public class BotUserRepository : IBotUserRepository
    {
        public BotUserRepository(BlogContext blogContext)
        {
            this.blogContext = blogContext;
        }

        private BlogContext blogContext;

        public async Task<List<BotUserShortData>> GetAllBotUsersAsync()
        {
            IEnumerable<BotUser> botUsers = blogContext.BotUsers.AsNoTracking();

            if (botUsers.Count() > 0)
            {
                List<BotUserShortData> userShortDatas = new List<BotUserShortData>();

                foreach (BotUser botUser in botUsers)
                {
                    userShortDatas.Add(new BotUserShortData()
                    {
                        Id = botUser.Id,
                        TelegramAccountId = botUser.TelegramAccountId,
                        UserName = botUser.UserName,
                        FirstName = botUser.FirstName,
                        LastName = botUser.LastName,
                        Link = botUser.Link
                    });
                }

                return userShortDatas;
            }
            else
            {
                throw new InvalidOperationException($"Bot users list empty");
            }
        }

        public async Task<BotUserShortData> GetBotUserByTelegramIdAsync(long telegramId)
        {
            BotUser? botUser = await blogContext.BotUsers.
                AsNoTracking().
                FirstOrDefaultAsync(b => b.TelegramAccountId == telegramId);

            if (botUser != null)
            {
                return new BotUserShortData()
                {
                    Id = botUser.Id,
                    TelegramAccountId = botUser.TelegramAccountId,
                    UserName = botUser.UserName,
                    FirstName = botUser.FirstName,
                    LastName = botUser.LastName,
                    Link = botUser.Link
                };
            }
            else
            {
                throw new InvalidOperationException($"Bot user with telegram id: ${telegramId} was null");
            }
        }

        public async Task<BotUserShortData> GetBotUserByIdAsync(long id)
        {
            BotUser? botUser = await blogContext.BotUsers.
                 AsNoTracking().
                 FirstOrDefaultAsync(b => b.Id == id);

            if (botUser != null)
            {
                return new BotUserShortData()
                {
                    Id = botUser.Id,
                    TelegramAccountId = botUser.TelegramAccountId,
                    UserName = botUser.UserName,
                    FirstName = botUser.FirstName,
                    LastName = botUser.LastName,
                    Link = botUser.Link
                };
            }
            else
            {
                throw new InvalidOperationException($"Bot user with id: ${id} was null");
            }
        }

        public async Task<BotUserShortData> AddBotUserAsync(CreateBotUserRequest botUserRequest)
        {
            BotUser botUser = new BotUser()
            {
                TelegramAccountId = botUserRequest.TelegramAccountId,
                UserName = botUserRequest.UserName,
                FirstName = botUserRequest.FirstName,
                LastName = botUserRequest.LastName,
                Link = botUserRequest.Link
            };

            await blogContext.BotUsers.AddAsync(botUser);
            await blogContext.SaveChangesAsync();

            return new BotUserShortData()
            {
                Id = botUser.Id,
                TelegramAccountId = botUser.TelegramAccountId,
                UserName = botUser.UserName,
                FirstName = botUser.FirstName,
                LastName = botUser.LastName,
                Link = botUser.Link
            };
        }
    }
}
