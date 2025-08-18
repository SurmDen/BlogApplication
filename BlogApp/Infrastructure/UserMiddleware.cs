using BlogApp.Data;
using BlogApp.Data.Helpers;
using BlogApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BlogApp.Infrastructure
{
    public class UserMiddleware
    {
        private RequestDelegate next;

        public UserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, BlogContext blogContext)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                string? userName = context.User.Identity.Name;
                string? email = context.User.FindFirstValue(ClaimTypes.Email);

                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(email))
                {
                    User? user = await blogContext.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.Email == email);

                    if (user != null)
                    {
                        UserShortData data = new UserShortData(user.Id, user.UserName, user.Email);

                        var userDataString = JsonConvert.SerializeObject(data);

                        context.Session.SetString("current_user_data", userDataString);
                    }
                }
            }

            await next.Invoke(context);
        }
    }
}
