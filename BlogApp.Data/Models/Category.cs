using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Category
    {
        public long Id { get; set; }

        public string CategoryName { get; set; }

        public string Alias { get; set; }

        public List<Blog> Blogs { get; set; }

        public Language Language { get; set; }

        public long LanguageId { get; set; }
    }
}
