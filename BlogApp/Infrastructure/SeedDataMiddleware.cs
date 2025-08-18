using BlogApp.Data;
using BlogApp.Models;

namespace BlogApp.Infrastructure
{
    public class SeedDataMiddleware
    {
        public SeedDataMiddleware(RequestDelegate requestDelegate)
        {
            next = requestDelegate;
        }

        private RequestDelegate next;

        public async Task InvokeAsync(HttpContext context, BlogContext blogContext, ILogger<SeedDataMiddleware> logger)
        {
            try
            {
                logger.LogInformation("Start seeding titles");

                SeedData.Seed(blogContext);

                logger.LogInformation("Finish seeding titles");
            }
            catch(Exception e)
            {
                logger.LogError("Error occured while try to seed titles, message: {@mess}", e.Message);
            }

            await next.Invoke(context);
        }
    }
}
