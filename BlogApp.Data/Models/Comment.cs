using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Comment
    {
        public long Id { get; set; }

        public string TextContent { get; set; } = string.Empty;

        public BotUser? BotUser { get; set; }

        public long BotUserId { get; set; }

        public List<Blog> Blogs { get; set; }

        public List<CommentAnswer> CommentAnswers { get; set; }

        public List<CommentTranslation> CommentTranslations { get; set; }

        public Language Language { get; set; }

        public long LanguageId { get; set; }

        [NotMapped]
        public bool IsTranslated { get; set; } = false;

        [NotMapped]
        public bool IsNoNeedTranslateButton { get; set; } = false;
    }
}
