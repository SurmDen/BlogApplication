using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Paragraph
    {
        public long Id { get; set; }

        public string Text { get; set; }

        public string? Image { get; set; }

        [NotMapped]
        public string? ImageBase64String { get; set; }

        public Subsection? Subsection { get; set; }

        public long SubsectionId { get; set; }
    }
}
