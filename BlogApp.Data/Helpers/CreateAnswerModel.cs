using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Helpers
{
    public class CreateAnswerModel
    {
        public string Text { get; set; } = string.Empty;

        public long UserId { get; set; }

        public long CommentId { get; set; }

        public string LangCode { get; set; } = string.Empty;
    }
}
