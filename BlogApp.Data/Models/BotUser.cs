using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class BotUser
    {
        public long Id { get; set; }

        public long TelegramAccountId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Link { get; set; } = string.Empty;

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<CommentAnswer> CommentAnswers { get; set; } = new List<CommentAnswer>();
    }
}
