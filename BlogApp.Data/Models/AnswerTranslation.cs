using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class AnswerTranslation
    {
        public long Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public string LangCode { get; set; } = string.Empty;

        public CommentAnswer CommentAnswer { get; set; }

        public long CommentAnswerId { get; set; }
    }
}
