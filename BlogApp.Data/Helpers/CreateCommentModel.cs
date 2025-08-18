using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Helpers
{
    public class CreateCommentModel
    {
        public string Text { get; set; }

        public string BlogAlias { get; set; }

        public long UserId { get; set; }

        public string LangCode { get; set; }
    }
}
