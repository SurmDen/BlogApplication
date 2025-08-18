using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Subsection
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public List<Paragraph> Paragraphs { get; set; }

        public Section? Section { get; set; }

        public long SectionId { get; set; }
    }
}
