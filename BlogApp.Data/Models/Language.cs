using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Language
    {
        public long Id { get; set; }

        public string LangName { get; set; }

        public string FullLangName { get; set; }

        public string CountryCode { get; set; }

        public List<Blog> Blogs { get; set; }

        public List<Category> Categories { get; set; }

        public List<Comment> Comments { get; set; }

        public List<CommentAnswer> CommentAnswers { get; set; }

        public Title Title { get; set; }
    }
}
