using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Section
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public List<Subsection> Subsections { get; set; }

        public Blog? Blog { get; set; }

        public long BlogId { get; set; }
    }
}
