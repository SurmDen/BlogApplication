using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Title
    {
        public long Id { get; set; }

        public string HeadTitle { get; set; }

        public string DescriptionTitle { get; set; }

        public string WarningInfo { get; set; }

        public string CatTitle { get; set; }

        public string MainRef { get; set; }

        public string BlogsRef { get; set; }

        public string Publicated { get; set; }

        public string Updated { get; set; }

        public string Author { get; set; }

        public string Back { get; set; }

        public string Forward { get; set; }

        public string SamePosts { get; set; }

        public string SearchByTags { get; set; }

        public Language Language { get; set; }

        public long LanguageId { get; set; }
    }
}
