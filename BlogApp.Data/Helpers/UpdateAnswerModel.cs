using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Helpers
{
    public class UpdateAnswerModel
    {
        public string Text { get; set; }= string.Empty;

        public long AnswerId { get; set; }
    }
}
