using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Helpers
{
    public class TranslateTargetModel
    {
        public long TargetId { get; set; }

        public string Text { get; set; } = string.Empty;

        public string LangCode { get; set; } = string.Empty;
    }
}
