using BlogApp.Data.Models;
using System.Text;

namespace BlogApp.Infrastructure
{
    public class RSSGenerator
    {
        public string GenerateRssChannelString(List<Category> categories)
        {
            StringBuilder rssBuilder = new StringBuilder();

            rssBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            rssBuilder.AppendLine("<rss xmlns:yandex=\"http://news.yandex.ru\"\r\n     xmlns:media=\"http://search.yahoo.com/mrss/\"\r\n     xmlns:turbo=\"http://turbo.yandex.ru\"\r\n     version=\"2.0\">");
            rssBuilder.AppendLine("<channel>");
            rssBuilder.AppendLine("<title>Блог и база знаний сервиса bot-market</title>");
            rssBuilder.AppendLine("<link>https://bot-market.com/</link>");
            rssBuilder.AppendLine("<description>Данный блог содержит полезную информацию по созданию и управлению telegram ботами</description>");
            
            foreach (Category category in categories)
            {
                if (category.Blogs.Count() > 0)
                {
                    foreach (Blog blog in category.Blogs)
                    {
                        rssBuilder.AppendLine("<item turbo=\"true\">");
                        rssBuilder.AppendLine("<turbo:extendedHtml>true</turbo:extendedHtml>");
                        rssBuilder.AppendLine($"<link>https://bot-market.com/blog/{category.Alias}/{blog.Alias}/{category.Language.LangName}</link>");
                        rssBuilder.AppendLine("<turbo:content>");
                        rssBuilder.AppendLine("<![CDATA[");
                        rssBuilder.AppendLine("<header>");
                        rssBuilder.AppendLine($"<h1>{blog.Title}</h1>");
                        rssBuilder.AppendLine("<figure>");
                        if (!string.IsNullOrEmpty(blog.Image))
                        {
                            rssBuilder.AppendLine($"<img src=\"https://bot-market.com{blog.Image}\">");
                        }
                        else
                        {
                            rssBuilder.AppendLine($"<img src=\"https://bot-market.com/common_imgs/bot.png\">");
                        }
                        rssBuilder.AppendLine("</figure>");
                        rssBuilder.AppendLine($"<p>{blog.Description}</p>");
                        rssBuilder.AppendLine("</header>");
                        rssBuilder.AppendLine("<section>");
                        foreach (Section section in blog.Sections)
                        {
                            rssBuilder.AppendLine($"<h3>{section.Title}</h3>");

                            foreach (Subsection subsection in section.Subsections)
                            {
                                rssBuilder.AppendLine($"<h4>{subsection.Title}</h4>");

                                foreach (Paragraph paragraph in subsection.Paragraphs)
                                {
                                    rssBuilder.AppendLine($"<p>{paragraph.Text}</p>");

                                    if (paragraph.Image != "")
                                    {
                                        rssBuilder.AppendLine("<figure>");
                                        rssBuilder.AppendLine($"<img src=\"https://bot-market.com{paragraph.Image}\">");
                                        rssBuilder.AppendLine("</figure>");
                                    }
                                }
                            }
                        }
                        rssBuilder.AppendLine("</section>");
                        rssBuilder.AppendLine("]]>");
                        rssBuilder.AppendLine("</turbo:content>");
                        rssBuilder.AppendLine("</item>");
                    }
                }
            }

            rssBuilder.AppendLine("</channel>");
            rssBuilder.AppendLine("</rss>");


            return rssBuilder.ToString();
        }
    }
}
