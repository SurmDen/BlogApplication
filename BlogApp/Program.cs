using BlogApp.Data;
using BlogApp.Infrastructure;
using BlogApp.Interfaces;
using BlogApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File("logs.txt")
    .WriteTo.Console()
    .CreateLogger();

builder.Host.ConfigureLogging(logging =>
{
    logging.AddSerilog();
    logging.SetMinimumLevel(LogLevel.Information);
})
.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}); ;

builder.Services.AddRazorPages();

builder.Services.AddSession();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddCors();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/blog/adminpage/login";

    options.Cookie.IsEssential = true;

    options.Cookie.HttpOnly = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = false;
});

builder.Services.AddDbContext<BlogContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteConnection"]);
    options.EnableSensitiveDataLogging();
});

builder.Services.AddTransient<IBlogRepository, BlogRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<IBotUserRepository, BotUserRepository>();
builder.Services.AddTransient<IBlogTranslator, BlogTranslator>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var provider = scope.ServiceProvider;

//    try
//    {
//        var db = provider.GetRequiredService<BlogContext>();

//        var pendingMigrations = db.Database.GetPendingMigrations();

//        if (pendingMigrations.Any())
//        {
//            db.Database.Migrate();
//        }
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e.Message);
//    }
//}

app.UseSession();

app.UseMiddleware<CountryMiddleware>();

app.UseStatusCodePagesWithReExecute("/blog/error", "?code={0}");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger().UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/blog/swagger/v1/swagger.json", "Bot market API");
    options.DocumentTitle = "Bot market API";
});

app.UseStaticFiles();

app.MapGet("/blog/authpage/login", () =>
{
    var filePath = Path.Combine(builder.Environment.WebRootPath, "blog", "authpage", "index.html");

    Console.WriteLine($"redirecting: {filePath}");

    return Results.File(filePath, "text/html");
});

app.UseDefaultFiles();

app.UseCors(options =>
{
    options.AllowAnyOrigin();

    options.AllowAnyMethod();

    options.AllowAnyHeader();
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();
