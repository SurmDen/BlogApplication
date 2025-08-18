using BlogApp.Data.Models;
using System.Text;
using BlogApp.Data.Helpers;
using Newtonsoft.Json;
using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BlogApp.Services
{
    public class BlogTranslator : IBlogTranslator
    {
        public BlogTranslator(BlogContext blogContext, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            this.blogContext = blogContext;
            this.configuration = configuration;

            baseURL = configuration["YandexApi:baseURL"] ?? "";
            folderID = configuration["YandexApi:folderId"] ?? "";
            apiKey = configuration["YandexApi:apiKey"] ?? "";

            this.serviceProvider = serviceProvider;
        }

        private BlogContext blogContext;
        private IConfiguration configuration;
        private IServiceProvider serviceProvider;

        private readonly string baseURL = string.Empty;
        private readonly string folderID = string.Empty;
        private readonly string apiKey = string.Empty;

        public async Task TranslateVideoNameAsync(string blogAlias, string vName, string vPath)
        {
            try
            {
                List<Blog> blogs = blogContext.Blogs.Where(b => b.Alias == blogAlias).Include(b => b.Language).ToList();

                foreach (Blog blog in blogs)
                {
                    blog.VideoPath = vPath;

                    if (blog.Language.LangName == "ru")
                    {
                        blog.VideoName = vName;
                    }
                    else
                    {
                        blog.VideoName = TranslateAsync(vName, blog.Language.LangName).Result.text;
                    }

                    blogContext.Blogs.Update(blog);

                    await blogContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {

                throw new Exception("Ошибка перевода", e);
            }
        }

        public async Task MakeFullBlogsTranslationAsync(Blog blog, List<Language> languages)
        {
            List<Category> categories = await blogContext.Categories.AsNoTracking().ToListAsync();

            Category selectedCategory = categories.First(c => c.Id == blog.CategoryId);

            try
            {
                object locker = new object();

                CancellationTokenSource cts = new CancellationTokenSource();

                ParallelOptions parallelOptions = new ParallelOptions()
                {
                    CancellationToken = cts.Token
                };

                Parallel.ForEach(languages.Where(l => l.LangName != "ka" && l.LangName != "ko"), parallelOptions, (Language lan) =>
                {
                    try
                    {
                        Blog translatedBlog = new Blog();

                        try
                        {
                            Console.WriteLine($"title: {lan.LangName}");

                            translatedBlog.Title = TranslateAsync(blog.Title, lan.LangName).Result.text;

                            Console.WriteLine(translatedBlog.Title);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Ошибка перевода", e);

                        }

                        translatedBlog.Alias = blog.Alias;

                        try
                        {
                            translatedBlog.Description = string.IsNullOrEmpty(blog.Description) ? "" :
                                TranslateAsync(blog.Description, lan.LangName).Result.text;

                        }
                        catch (Exception e)
                        {
                            throw new Exception("Ошибка перевода", e);
                        }

                        translatedBlog.UserId = blog.UserId;

                        translatedBlog.DateOfPublish = blog.DateOfPublish;

                        translatedBlog.LanguageId = lan.Id;

                        translatedBlog.Image = string.IsNullOrEmpty(blog.Image) ? "" : blog.Image;

                        translatedBlog.CategoryId = categories.
                            First(c => c.Alias == selectedCategory.Alias && c.LanguageId == lan.Id).Id;

                        //if (blog.Tags != null)
                        //{
                        //    if (blog.Tags.Count() > 0)
                        //    {
                        //        translatedBlog.Tags = blog.Tags;
                        //    }
                        //}

                        foreach (Section section in blog.Sections)
                        {
                            if (translatedBlog.Sections == null)
                            {
                                translatedBlog.Sections = new List<Section>();
                            }

                            Section translatedSection = new Section();

                            try
                            {
                                translatedSection.Title = string.IsNullOrEmpty(section.Title) ? "" :
                                TranslateAsync(section.Title, lan.LangName).Result.text;
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Ошибка перевода", e);
                            }

                            translatedBlog.Sections.Add(translatedSection);

                            foreach (Subsection sub in section.Subsections)
                            {
                                if (translatedSection.Subsections == null)
                                {
                                    translatedSection.Subsections = new List<Subsection>();
                                }

                                Subsection translatedSub = new Subsection();

                                try
                                {
                                    translatedSub.Title = string.IsNullOrEmpty(sub.Title) ? "" :
                                    TranslateAsync(sub.Title, lan.LangName).Result.text;
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Ошибка перевода", e);
                                }

                                translatedSection.Subsections.Add(translatedSub);

                                foreach (Paragraph par in sub.Paragraphs)
                                {
                                    if (translatedSub.Paragraphs == null)
                                    {
                                        translatedSub.Paragraphs = new List<Paragraph>();
                                    }

                                    Paragraph translatedPar = new Paragraph()
                                    {
                                        Image = string.IsNullOrEmpty(par.Image) ? "" : par.Image
                                    };

                                    try
                                    {
                                        translatedPar.Text = string.IsNullOrEmpty(par.Text) ? "" :
                                        TranslateAsync(par.Text, lan.LangName).Result.text;
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception("Ошибка перевода", e);
                                    }

                                    translatedSub.Paragraphs.Add(translatedPar);
                                }
                            }
                        }

                        lock(locker){
                            blogContext.Blogs.Add(translatedBlog);
                        }
                    }
                    catch (Exception)
                    {
                        //cts.Cancel();
                    }
                });
            }
            catch (Exception e)
            {
                await blogContext.SaveChangesAsync();

                throw new Exception("Перевод статьи аварийно завершен", e);
            }

            await blogContext.SaveChangesAsync();
        }

        public async Task TranslateNewBlogAsync(Blog blog)
        {

            List<Category> categories = await blogContext.Categories.AsNoTracking().ToListAsync();

            List<Language> languages = await blogContext.Languages.AsNoTracking().ToListAsync();

            List<Blog> blogsFromDb = await blogContext.Blogs.
                Include(b => b.Language).
                Where(b => b.Language.LangName == "ru").
                ToListAsync();

            AliasMaker aliasMaker = new AliasMaker();

            Category selectedCategory = categories.First(c => c.Id == blog.CategoryId);

            Translation titleTranslation;

            try
            {
                titleTranslation = await TranslateAsync(blog.Title, "en");
            }
            catch (Exception e)
            {
                Console.WriteLine("alias error (create blog)");

                throw new Exception("Ошибка перевода", e);
            }

            string alias = aliasMaker.GenerateAlias(titleTranslation.text);

            if (blogsFromDb.Count() > 0)
            {
                int sameAliasCount = 0;

                foreach (var b in blogsFromDb)
                {
                    if (b.Alias.Contains(alias))
                    {
                        sameAliasCount++;
                    }
                }

                if (sameAliasCount > 0)
                {
                    alias = alias + $"-{sameAliasCount}";
                }
            }

            Language detectedLang = languages.First(l => l.LangName == "ru");

            blog.LanguageId = detectedLang.Id;

            blog.Alias = alias;

            blogContext.Add(blog);

            await blogContext.SaveChangesAsync();

            try
            {
                object locker = new object();

                CancellationTokenSource cts = new CancellationTokenSource();

                ParallelOptions parallelOptions = new ParallelOptions()
                {
                    CancellationToken = cts.Token
                };

                Parallel.ForEach(languages.Where(l => l.Id != detectedLang.Id), parallelOptions, (Language lan) =>
                {

                    try
                    {
                        Console.WriteLine($"{lan.LangName}_task started");

                        Blog translatedBlog = new Blog();

                        try
                        {
                            translatedBlog.Title = TranslateAsync(blog.Title, lan.LangName).Result.text;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Ошибка перевода", e);
                        }

                        translatedBlog.Alias = alias;

                        try
                        {
                            translatedBlog.Description = string.IsNullOrEmpty(blog.Description) ? "" :
                                TranslateAsync(blog.Description, lan.LangName).Result.text;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Ошибка перевода", e);
                        }

                        translatedBlog.UserId = blog.UserId;

                        translatedBlog.Tags = blog.Tags;

                        translatedBlog.DateOfPublish = blog.DateOfPublish;

                        translatedBlog.LanguageId = lan.Id;

                        translatedBlog.Image = string.IsNullOrEmpty(blog.Image) ? "" : blog.Image;

                        translatedBlog.CategoryId = categories.
                            First(c => c.Alias == selectedCategory.Alias && c.LanguageId == lan.Id).Id;

                        foreach (Section section in blog.Sections)
                        {
                            if (translatedBlog.Sections == null)
                            {
                                translatedBlog.Sections = new List<Section>();
                            }

                            Section translatedSection = new Section();

                            try
                            {
                                translatedSection.Title = string.IsNullOrEmpty(section.Title) ? "" :
                                TranslateAsync(section.Title, lan.LangName).Result.text;
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Ошибка перевода", e);
                            }

                            translatedBlog.Sections.Add(translatedSection);

                            foreach (Subsection sub in section.Subsections)
                            {
                                if (translatedSection.Subsections == null)
                                {
                                    translatedSection.Subsections = new List<Subsection>();
                                }

                                Subsection translatedSub = new Subsection();

                                try
                                {
                                    translatedSub.Title = string.IsNullOrEmpty(sub.Title) ? "" :
                                    TranslateAsync(sub.Title, lan.LangName).Result.text;
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Ошибка перевода", e);
                                }

                                translatedSection.Subsections.Add(translatedSub);

                                foreach (Paragraph par in sub.Paragraphs)
                                {
                                    if (translatedSub.Paragraphs == null)
                                    {
                                        translatedSub.Paragraphs = new List<Paragraph>();
                                    }

                                    Paragraph translatedPar = new Paragraph()
                                    {
                                        Image = string.IsNullOrEmpty(par.Image) ? "" : par.Image
                                    };

                                    try
                                    {
                                        translatedPar.Text = string.IsNullOrEmpty(par.Text) ? "" :
                                        TranslateAsync(par.Text, lan.LangName).Result.text;
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception("Ошибка перевода", e);
                                    }

                                    translatedSub.Paragraphs.Add(translatedPar);
                                }
                            }
                        }

                        lock (locker)
                        {
                            blogContext.Blogs.Add(translatedBlog);
                        }
                    }
                    catch (Exception e)
                    {
                        cts.Cancel();
                    }
                });

            }
            catch (Exception e)
            {   
                await blogContext.SaveChangesAsync();

                throw new Exception("Перевод статьи аварийно завершен", e);
            }

            await blogContext.SaveChangesAsync();
        }

        public async Task UpdateBlogsTranslationAsync(Blog updatedBlog)
        {
            List<Blog> allBlogs =
                    blogContext.Blogs.
                    Where(b => b.Alias == updatedBlog.Alias).
                    Include(b => b.Language).
                    Include(b => b.Sections).
                    ThenInclude(s => s.Subsections).
                    ThenInclude(s => s.Paragraphs).
                    ToList();

            Blog updatedBlogFromDb = allBlogs.First(b => b.Id == updatedBlog.Id);

            bool isBlogTitleUpdated = false;
            bool isBlogDescriptionUpdated = false;

            List<long> updatedSectionTitleIds = new List<long>();
            List<long> updatedSubsectionTitleIds = new List<long>();
            List<long> updatedParagraphIds = new List<long>();

            if (!string.IsNullOrEmpty(updatedBlog.Image))
            {
                updatedBlogFromDb.Image = updatedBlog.Image;
            }

            if (updatedBlogFromDb.Title != updatedBlog.Title)
            {
                updatedBlogFromDb.Title = updatedBlog.Title;

                isBlogTitleUpdated = true;
            }

            if (updatedBlogFromDb.Description != updatedBlog.Description)
            {
                updatedBlogFromDb.Description = updatedBlog.Description;

                isBlogDescriptionUpdated = true;
            }

            updatedBlogFromDb.DateOfUpdate = updatedBlog.DateOfUpdate;


            //Removing elements for all translations of blog

            foreach (var b in allBlogs)
            {
                int updatedBlogSectionsCount = updatedBlog.Sections.Count();

                int updatedBlogFromDbSectionsCount = b.Sections.Count();

                int sectionsDef = updatedBlogFromDbSectionsCount - updatedBlogSectionsCount;

                if (sectionsDef > 0)
                {
                    IEnumerable<Section> deletedSections = b.Sections.Skip(updatedBlogSectionsCount).Take(sectionsDef);

                    foreach (Section section in deletedSections)
                    {
                        b.Sections.Remove(section);
                    }
                }

                for (int i = 0; i < b.Sections.Count; i++)
                {
                    int dbSubsectionsCount = b.Sections[i].Subsections.Count();

                    int subsectionsCount = updatedBlog.Sections[i].Subsections.Count();

                    int subsectionsDef = dbSubsectionsCount - subsectionsCount;

                    if (subsectionsDef > 0)
                    {
                        IEnumerable<Subsection> deletedSubsections = b.Sections[i].Subsections.
                            Skip(subsectionsCount).
                            Take(subsectionsDef);

                        foreach (var sub in deletedSubsections)
                        {
                            b.Sections[i].Subsections.Remove(sub);
                        }
                    }

                    for (int j = 0; j < b.Sections[i].Subsections.Count; j++)
                    {
                        int dbparCount = b.Sections[i].Subsections[j].Paragraphs.Count();

                        int parCount = updatedBlog.Sections[i].Subsections[j].Paragraphs.Count();

                        int parDef = dbparCount - parCount;

                        if (parDef > 0)
                        {
                            IEnumerable<Paragraph> deletedPars = b.Sections[i].Subsections[j].Paragraphs.
                                Skip(parCount).
                                Take(parDef);

                            foreach (var par in deletedPars)
                            {
                                b.Sections[i].Subsections[j].Paragraphs.Remove(par);
                            }
                        }
                    }
                }
            }



            //Adding elements

            for (int i = 0; i < updatedBlog.Sections.Count; i++)
            {
                if (i + 1 > updatedBlogFromDb.Sections.Count())
                {
                    updatedBlogFromDb.Sections.Add(updatedBlog.Sections[i]);

                    continue;
                }
                else
                {
                    if (updatedBlogFromDb.Sections[i].Title != updatedBlog.Sections[i].Title)
                    {
                        updatedBlogFromDb.Sections[i].Title = updatedBlog.Sections[i].Title;

                        updatedSectionTitleIds.Add(updatedBlogFromDb.Sections[i].Id);
                    }
                }

                for (int j = 0; j < updatedBlog.Sections[i].Subsections.Count; j++)
                {
                    if (j + 1 > updatedBlogFromDb.Sections[i].Subsections.Count())
                    {
                        updatedBlogFromDb.Sections[i].Subsections.Add(updatedBlog.Sections[i].Subsections[j]);

                        continue;
                    }
                    else
                    {
                        if (updatedBlogFromDb.Sections[i].Subsections[j].Title != updatedBlog.Sections[i].Subsections[j].Title)
                        {
                            updatedBlogFromDb.Sections[i].Subsections[j].Title = updatedBlog.Sections[i].Subsections[j].Title;

                            updatedSubsectionTitleIds.Add(updatedBlogFromDb.Sections[i].Subsections[j].Id);
                        }
                    }

                    for (int k = 0; k < updatedBlog.Sections[i].Subsections[j].Paragraphs.Count; k++)
                    {
                        if (k + 1 > updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs.Count())
                        {
                            updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs.
                                Add(updatedBlog.Sections[i].Subsections[j].Paragraphs[k]);

                            continue;
                        }
                        else
                        {
                            if (updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Text != updatedBlog.Sections[i].Subsections[j].Paragraphs[k].Text)
                            {
                                updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Text =
                                updatedBlog.Sections[i].Subsections[j].Paragraphs[k].Text;

                                updatedParagraphIds.Add(updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Id);
                            }

                            if (!string.IsNullOrEmpty(updatedBlog.Sections[i].Subsections[j].Paragraphs[k].Image))
                            {
                                updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Image =
                                updatedBlog.Sections[i].Subsections[j].Paragraphs[k].Image;
                            }
                        }
                    }
                }
            }

            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();

                ParallelOptions parallelOptions = new ParallelOptions()
                {
                    CancellationToken = cts.Token
                };

                Parallel.ForEach(allBlogs.Where(b => b.Id != updatedBlogFromDb.Id), parallelOptions, (Blog blog) =>
                {
                    try
                    {

                        string langCode = blog.Language.LangName;

                        if (isBlogTitleUpdated)
                        {
                            try
                            {
                                blog.Title = TranslateAsync(updatedBlogFromDb.Title, langCode).Result.text;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Ошибка перевода 1, детали: {e.Message}");
                                throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                            }
                        }

                        if (isBlogDescriptionUpdated)
                        {
                            try
                            {
                                blog.Description = string.IsNullOrEmpty(updatedBlogFromDb.Description) ? "" :
                                    TranslateAsync(updatedBlogFromDb.Description, langCode).Result.text;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Ошибка перевода 2, детали: {e.Message}");
                                throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                            }
                        }

                        blog.Image = updatedBlogFromDb.Image;

                        for (int i = 0; i < updatedBlogFromDb.Sections.Count; i++)
                        {
                            if (i + 1 > blog.Sections.Count())
                            {
                                Section newBlogsection = new Section();

                                newBlogsection.Subsections = new List<Subsection>();

                                blog.Sections.Add(newBlogsection);

                                foreach (var sub in updatedBlogFromDb.Sections[i].Subsections)
                                {
                                    Subsection newBlogSubsection = new Subsection();

                                    newBlogSubsection.Paragraphs = new List<Paragraph>();

                                    newBlogsection.Subsections.Add(newBlogSubsection);

                                    foreach (var p in sub.Paragraphs)
                                    {
                                        Paragraph newBlogParagraph = new Paragraph();

                                        newBlogParagraph.Image = p.Image;

                                        newBlogSubsection.Paragraphs.Add(newBlogParagraph);
                                    }
                                }

                                continue;
                            }



                            for (int j = 0; j < updatedBlogFromDb.Sections[i].Subsections.Count; j++)
                            {
                                if (j + 1 > blog.Sections[i].Subsections.Count())
                                {
                                    Subsection newBlogSubsection = new Subsection();

                                    newBlogSubsection.Paragraphs = new List<Paragraph>();

                                    blog.Sections[i].Subsections.Add(newBlogSubsection);

                                    foreach (var p in updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs)
                                    {
                                        Paragraph newBlogPar = new Paragraph();

                                        newBlogPar.Image = p.Image;

                                        newBlogSubsection.Paragraphs.Add(newBlogPar);
                                    }

                                    continue;
                                }

                                for (int k = 0; k < updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs.Count; k++)
                                {
                                    if (k + 1 > blog.Sections[i].Subsections[j].Paragraphs.Count())
                                    {
                                        Paragraph newBlogPar = new Paragraph();

                                        newBlogPar.Image = updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Image;

                                        blog.Sections[i].Subsections[j].Paragraphs.Add(newBlogPar);

                                        continue;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < blog.Sections.Count; i++)
                        {

                            if (updatedBlogFromDb.Sections[i].Id > 0)
                            {
                                if (updatedSectionTitleIds.Contains(updatedBlogFromDb.Sections[i].Id))
                                {
                                    try
                                    {
                                        blog.Sections[i].Title =
                                            TranslateAsync(updatedBlogFromDb.Sections[i].Title, langCode).Result.text;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine($"Ошибка перевода 3, детали: {e.Message}");
                                        throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    blog.Sections[i].Title =
                                        TranslateAsync(updatedBlogFromDb.Sections[i].Title, langCode).Result.text;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Ошибка перевода 4, детали: {e.Message}");
                                    throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                                }
                            }


                            for (int j = 0; j < blog.Sections[i].Subsections.Count; j++)
                            {
                                if (updatedBlogFromDb.Sections[i].Subsections[j].Id > 0)
                                {
                                    if (updatedSubsectionTitleIds.Contains(updatedBlogFromDb.Sections[i].Subsections[j].Id))
                                    {
                                        try
                                        {
                                            blog.Sections[i].Subsections[j].Title =
                                                TranslateAsync(updatedBlogFromDb.Sections[i].Subsections[j].Title, langCode).Result.text;
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine($"Ошибка перевода 5, детали: {e.Message}");
                                            throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        blog.Sections[i].Subsections[j].Title =
                                            TranslateAsync(updatedBlogFromDb.Sections[i].Subsections[j].Title, langCode).Result.text;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine($"Ошибка перевода 6, детали: {e.Message}");
                                        throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                                    }
                                }

                                for (int k = 0; k < blog.Sections[i].Subsections[j].Paragraphs.Count; k++)
                                {
                                    if (updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Id > 0)
                                    {
                                        if (updatedParagraphIds.Contains(updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Id))
                                        {
                                            try
                                            {
                                                blog.Sections[i].Subsections[j].Paragraphs[k].Text =
                                                    TranslateAsync(updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Text, langCode).Result.text;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Ошибка перевода 7, детали: {e.Message}");
                                                throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            blog.Sections[i].Subsections[j].Paragraphs[k].Text =
                                                TranslateAsync(updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Text, langCode).Result.text;
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine($"Ошибка перевода 8, детали: {e.Message}");
                                            throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                                        }
                                    }

                                    try
                                    {
                                        blog.Sections[i].Subsections[j].Paragraphs[k].Text =
                                            TranslateAsync(updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Text, langCode).Result.text;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine($"Ошибка перевода 9, детали: {e.Message}");
                                        throw new Exception($"Ошибка перевода, детали: {e.Message}", e);
                                    }

                                    blog.Sections[i].Subsections[j].Paragraphs[k].Image = updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Image;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        cts.Cancel();
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка перевода 10, детали: {e.Message}");
                throw new Exception($"Обновление перевода статьи аварийно завершено, детали: {e.Message}", e);
            }

            blogContext.UpdateRange(allBlogs);

            await blogContext.SaveChangesAsync();
        }

        public async Task UpdateSpecificBlogsTranslationAsync(Blog updatedBlog, Blog updatedBlogFromDb)
        {

            if (!string.IsNullOrEmpty(updatedBlog.Image))
            {
                updatedBlogFromDb.Image = updatedBlog.Image;
            }

            if (updatedBlogFromDb.Title != updatedBlog.Title)
            {
                updatedBlogFromDb.Title = updatedBlog.Title;
            }

            if (updatedBlogFromDb.Description != updatedBlog.Description)
            {
                updatedBlogFromDb.Description = updatedBlog.Description;
            }

            updatedBlogFromDb.DateOfUpdate = updatedBlog.DateOfUpdate;

            try
            {
                for (int i = 0; i < updatedBlog.Sections.Count; i++)
                {
                    updatedBlogFromDb.Sections[i].Title = updatedBlog.Sections[i].Title;

                    for (int j = 0; j < updatedBlog.Sections[i].Subsections.Count; j++)
                    {
                        updatedBlogFromDb.Sections[i].Subsections[j].Title = updatedBlog.Sections[i].Subsections[j].Title;

                        for (int k = 0; k < updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs.Count; k++)
                        {
                            updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Text =
                                updatedBlog.Sections[i].Subsections[j].Paragraphs[k].Text;

                            if (!string.IsNullOrEmpty(updatedBlog.Sections[i].Subsections[j].Paragraphs[k].Image))
                            {
                                updatedBlogFromDb.Sections[i].Subsections[j].Paragraphs[k].Image =
                                updatedBlog.Sections[i].Subsections[j].Paragraphs[k].Image;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Обновление перевода статьи аварийно завершено", e);
            }
        }

        public async Task TranslateNewCategoryAsync(Category category)
        {
            List<Language> languages = await blogContext.Languages.AsNoTracking().ToListAsync();

            List<Category> TranslatedCategories = new List<Category>();

            AliasMaker aliasMaker = new AliasMaker();

            Translation translation = await TranslateAsync(category.CategoryName, "en");

            string categoryAlias = aliasMaker.GenerateAlias(translation.text);

            category.Alias = categoryAlias;

            Language detectedLanguage = languages.First(l => l.LangName == "ru");

            category.LanguageId = detectedLanguage.Id;

            TranslatedCategories.Add(category);

            foreach (Language language in languages.
                Where(l => l.Id != detectedLanguage.Id && l.LangName != "ka" && l.LangName != "ko"))
            {
                Category translatedCategory = new Category()
                {
                    Alias = categoryAlias,

                    LanguageId = language.Id
                };

                try
                {
                    Console.WriteLine("Category lan: "  +  language.LangName);

                    translatedCategory.CategoryName = string.IsNullOrEmpty(category.CategoryName) ? ""
                    : TranslateAsync(category.CategoryName, language.LangName).Result.text;

                    Console.WriteLine(translatedCategory.CategoryName);
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка перевода", e);
                }

                TranslatedCategories.Add(translatedCategory);
            }

            await blogContext.Categories.AddRangeAsync(TranslatedCategories);

            await blogContext.SaveChangesAsync();
        }

        public async Task UpdateCategoriesTranslationAsync(Category updatedCategory)
        {
            List<Category> allCategories = blogContext.Categories.
                Where(c => c.Alias == updatedCategory.Alias).
                Include(c => c.Language).
                ToList();

            foreach (Category category in allCategories.Where(c => c.Id != updatedCategory.Id))
            {

                try
                {
                    category.CategoryName =
                    TranslateAsync(updatedCategory.CategoryName, category.Language.LangName).Result.text;

                    blogContext.Update(category);

                    await Console.Out.WriteLineAsync($"updated category:   {category.CategoryName}");
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка перевода", e);
                }
            }

            Category oldCategoryFromDb = allCategories.First(c => c.Id == updatedCategory.Id);

            oldCategoryFromDb.CategoryName = updatedCategory.CategoryName;

            blogContext.Categories.Update(oldCategoryFromDb);

            await blogContext.SaveChangesAsync();
        }

        Semaphore semaphore = new Semaphore(19,19);

        public async Task<Translation> TranslateAsync(string text, string langCode)
        {

            HttpClient client = new HttpClient();

            client.Timeout = TimeSpan.FromMinutes(1);

            using StringContent jsonContent = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        format = "HTML",
                        targetLanguageCode = langCode,
                        texts = new string[] { text },
                        folderId = folderID,
                    }), Encoding.UTF8,
                    "application/json");


            HttpRequestMessage request = new HttpRequestMessage();

            request.RequestUri = new Uri(baseURL);

            request.Content = jsonContent;

            request.Method = HttpMethod.Post;

            request.Headers.Add("Authorization", $"Api-Key {apiKey}");

            HttpResponseMessage response;

            semaphore.WaitOne();
            
            response = client.Send(request);

            Thread.Sleep(1000);
            
            semaphore.Release();

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();

                Root root = JsonConvert.DeserializeObject<Root>(jsonResponse);

                if (root != null)
                {
                    return root.translations.First();
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            else
            {
                throw new BadHttpRequestException(await response.Content.ReadAsStringAsync(), (int)response.StatusCode);
            }

        }

    }
}
