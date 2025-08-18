using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class CommentAnswer
    {
        public long Id { get; set; }

        public string TextContent { get; set; } = string.Empty;

        public List<AnswerTranslation> AnswerTranslations { get; set; }

        public Comment? Comment { get; set; }

        public long CommentId { get; set; }

        public BotUser? BotUser { get; set; }

        public long BotUserId { get; set; }

        public Language Language { get; set; }

        public long LanguageId { get; set; }

        [NotMapped]
        public bool IsTranslated { get; set; } = false;

        [NotMapped]
        public bool IsNoNeedTranslateButton { get; set; } = false;
    }
}
