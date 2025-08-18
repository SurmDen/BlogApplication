using BlogApp.Data.Helpers;
using BlogApp.Data.Models;

namespace BlogApp.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> CreateUserAsync(CreateUserModel userModel);

        public Task UpdateUserAsync(User user);

        public Task<bool> IsUserWithNameExistsAsync(string userName);

        public Task<bool> IsUserWithEmailExistsAsync(string userEmail);

        public Task<bool> IsUserWithPasswordExistsAsync(string userPassword);

        public Task DeleteUserAsync(long userId);

        public Task<User> GetUserByIdAsync(long userId);

        public Task<User> GetUserByNameAndPasswordAsync(SigningData signingData);

        public Task<List<User>> GetUsersAsync();
    }
}
