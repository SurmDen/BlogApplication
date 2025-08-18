using BlogApp.Data;
using BlogApp.Data.Models;
using BlogApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Models
{
    public class SeedData
    {
        public static void Seed(BlogContext blogContext)
        {
            //if (!blogContext.Titles.Any())
            //{
            //    IEnumerable<Language> languages = blogContext.Languages;

            //    if (languages == null)
            //    {
            //        return;
            //    }

            //    if (languages.Count() > 0)
            //    {
            //        Title rusTitle = new Title()
            //        {
            //            Id = 1,
            //            HeadTitle = "Блог и база знаний сервиса",
            //            DescriptionTitle = "Все, что нужно знать для создания ботов",
            //            WarningInfo = "Информация на сайте предоставлена для ознакомления, администрация сайта не несет ответственности за использование размещенной на сайте информации.",
            //            CatTitle = "Категории",
            //            MainRef = "Главная",
            //            BlogsRef = "Все статьи",
            //            Publicated = "Опубликовано",
            //            Updated = "Обновлено",
            //            Author = "Автор",
            //            Back = "Назад",
            //            Forward = "Далее",
            //            LanguageId = 1
            //        };

            //        List<Title> titles = new List<Title>() { rusTitle };

            //        BlogTranslator translator = new BlogTranslator();

            //        foreach (Language language in languages.Where(l => l.LangName != "ru"))
            //        {
            //            var title = new Title()
            //            {
            //                Id = language.Id,
            //                LanguageId = language.Id,
            //                HeadTitle = translator.TranslateAsync(rusTitle.HeadTitle, language.LangName).Result.text,
            //                DescriptionTitle = translator.TranslateAsync(rusTitle.DescriptionTitle, language.LangName).Result.text,
            //                WarningInfo = translator.TranslateAsync(rusTitle.WarningInfo, language.LangName).Result.text,
            //                CatTitle = translator.TranslateAsync(rusTitle.CatTitle, language.LangName).Result.text,
            //                MainRef = translator.TranslateAsync(rusTitle.MainRef, language.LangName).Result.text,
            //                BlogsRef = translator.TranslateAsync(rusTitle.BlogsRef, language.LangName).Result.text,
            //                Publicated = translator.TranslateAsync(rusTitle.Publicated, language.LangName).Result.text,
            //                Updated = translator.TranslateAsync(rusTitle.Updated, language.LangName).Result.text,
            //                Author = translator.TranslateAsync(rusTitle.Author, language.LangName).Result.text,
            //                Back = translator.TranslateAsync(rusTitle.Back, language.LangName).Result.text,
            //                Forward = translator.TranslateAsync(rusTitle.Forward, language.LangName).Result.text
            //            };

            //            titles.Add(title);
            //        }

            //        blogContext.Titles.AddRange(titles);
            //        blogContext.SaveChanges();
            //    }
            //}

            //string sameStatesTitle = "поиск по тегам";

            //IEnumerable<Title> allTitles = blogContext.Titles.Include(t => t.Language);

            //BlogTranslator translator = new BlogTranslator();

            //foreach (Title t in allTitles)
            //{
            //    if (t.Language.LangName == "ru")
            //    {
            //        t.SearchByTags = sameStatesTitle;
            //    }
            //    else
            //    {
            //        t.SearchByTags = translator.TranslateAsync(sameStatesTitle, t.Language.LangName).Result.text;
            //    }

            //    blogContext.Titles.Update(t);
            //}

            //blogContext.SaveChanges();
        }
    }
}
