using System.Net;
using System.Runtime;
using BlogApp.Data;
using BlogApp.Data.Helpers;
using BlogApp.Data.Models;
using BlogApp.Interfaces;
using Newtonsoft.Json;

namespace BlogApp.Infrastructure
{
    public class CountryMiddleware
    {
        public CountryMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        private readonly RequestDelegate next;

        public async Task InvokeAsync(HttpContext context, BlogContext blogContext)
        {
            try
            {
                IPAddress? clientIpAddress = context.Connection.RemoteIpAddress;

                string? lang = context.Session.GetString("lang");

                if (string.IsNullOrEmpty(lang))
                {
                    await Console.Out.WriteLineAsync("it's work!!!");

                    if (clientIpAddress != null)
                    {
                        using (var client = new HttpClient())
                        {
                            HttpResponseMessage responseMessage = await client.GetAsync("http://ipinfo.io/" + clientIpAddress.ToString());

                            if (responseMessage.IsSuccessStatusCode)
                            {
                                IpInfo ipInfo = JsonConvert.DeserializeObject<IpInfo>(await responseMessage.Content.ReadAsStringAsync());

                                if (!string.IsNullOrEmpty(ipInfo.country))
                                {
                                    string countryCode = ipInfo.country.ToLower();

                                    if (countryCode == "by")
                                    {
                                        countryCode = "ru";
                                    }

                                    IEnumerable<Language> languages = blogContext.Languages;

                                    string langCode = "ru";

                                    foreach (var l in languages)
                                    {
                                        if (l.CountryCode == countryCode)
                                        {
                                            langCode = l.LangName;
                                        }
                                    }

                                    context.Session.SetString("lang", langCode);

                                }
                                else
                                {
                                    context.Session.SetString("lang", "ru");
                                }
                            }
                            else
                            {
                                context.Session.SetString("lang", "ru");
                            }
                        }
                    }
                    else
                    {
                        context.Session.SetString("lang", "ru");
                    }
                }
            }
            catch
            {
                context.Session.SetString("lang", "ru");
            }

            await next.Invoke(context);
        }
    }
}
