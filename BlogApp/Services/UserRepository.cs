using BlogApp.Data;
using BlogApp.Data.Helpers;
using BlogApp.Data.Models;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(BlogContext blogContext)
        {
            this.blogContext = blogContext;
        }

        private BlogContext blogContext;

        public async Task<User> CreateUserAsync(CreateUserModel userModel)
        {
            User user = new User()
            {
                UserName = userModel.UserName,

                Email = userModel.Email,

                Password = userModel.Password
            };

            await blogContext.Users.AddAsync(user);

            Role? role = await blogContext.Roles.FirstOrDefaultAsync(r => r.RoleName.ToLower() == "user");

            if (role != null)
            {
                user.RoleId = role.Id;
                user.Role = role;
            }
            else
            {
                throw new NullReferenceException("User role data is not exists");
            }

            await blogContext.SaveChangesAsync();

            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            blogContext.Users.Update(user);

            await blogContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(long userId)
        {
            User user = await blogContext.Users.
                Include(u => u.Blogs).
                ThenInclude(b => b.Sections).
                ThenInclude(s => s.Subsections).
                ThenInclude(sub => sub.Paragraphs).FirstAsync(u => u.Id == userId);

            blogContext.Users.Remove(user);

            await blogContext.SaveChangesAsync();
        }

        public async Task<User> GetUserByIdAsync(long userId)
        {
            User user = await blogContext.Users.FirstAsync(u => u.Id == userId);

            return user;
        }

        public async Task<User> GetUserByNameAndPasswordAsync(SigningData signingData)
        {
            User? user;

            try
            {
                user = await blogContext.Users.
                    Include(u => u.Blogs).
                    Include(u => u.Role).
                    FirstAsync(u => u.UserName == signingData.UserName && u.Password == signingData.Password);
            }
            catch
            {
                user = null;
            }

            if (user != null)
            {
                foreach (Blog blog in user.Blogs)
                {
                    blog.User = null;
                }

                user.Role.Users = null;
            }

            return user;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            List<User> users = await blogContext.Users.
                Include(u => u.Blogs).ToListAsync();

            foreach (User user in users)
            {
                foreach (Blog blog in user.Blogs)
                {
                    blog.User = null;
                }
            }

            return users;
        }

        public async Task<bool> IsUserWithNameExistsAsync(string userName)
        {
            User? user = await blogContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> IsUserWithEmailExistsAsync(string userEmail)
        {
            User? user = await blogContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> IsUserWithPasswordExistsAsync(string userPassword)
        {
            User? user = await blogContext.Users.FirstOrDefaultAsync(u => u.Password == userPassword);

            if (user == null)
            {
                return false;
            }

            return true;
        }
    }
}
