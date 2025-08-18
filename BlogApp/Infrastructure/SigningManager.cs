using BlogApp.Data.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace BlogApp.Infrastructure
{
    public class SigningManager
    {
        public async Task LoginAsync(SigningModel signingModel, HttpContext httpContext)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, signingModel.UserName),

                new Claim(ClaimTypes.Email, signingModel.UserEmail),

                new Claim(ClaimTypes.Role, signingModel.UserRole)
            };

            ClaimsIdentity identity =
                new ClaimsIdentity(claims, "ApplicationCookie", ClaimTypes.Name, ClaimTypes.Role);


            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        public async Task LogoutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
