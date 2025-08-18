using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Tag
    {
        public long Id { get; set; }

        public string TagName { get; set; }

        public List<Blog> Blogs { get; set; }
    }
}
