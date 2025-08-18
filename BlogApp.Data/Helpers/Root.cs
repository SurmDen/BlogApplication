using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Helpers
{
    public class Root
    {
        public List<Translation> translations { get; set; }
    }

    public class Translation
    {
        public string text { get; set; }

        public string detectedLanguageCode { get; set; }
    }
}
