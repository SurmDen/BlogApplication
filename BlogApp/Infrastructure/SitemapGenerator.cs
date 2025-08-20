using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace BlogApp.Infrastructure
{
    public class SitemapGenerator
    {
        public string GenerateDocument(List<string> nodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            XElement root = new XElement(xmlns + "urlset");

            foreach (string sitemapNode in nodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", sitemapNode),
                    new XElement(xmlns + "isMultilanguage", "true"),
                    new XElement(xmlns + "lastmod", DateTime.Today.ToString("yyyy-MM-dd")));

                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);

            return document.ToString();
        }
    }
}
