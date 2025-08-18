using System.Text;

namespace BlogApp.Infrastructure
{
    public class AliasMaker
    {
        public string GenerateAlias(string blogTitle)
        {
            string baseItems = "abcdefghijklmnopqrstuvwxyz";

            StringBuilder aliasBuilder = new StringBuilder();

            char[] blogTitleItems = blogTitle.ToLower().ToCharArray();

            foreach (char blogTitleItem in blogTitleItems)
            {
                if (baseItems.Contains(blogTitleItem))
                {
                    aliasBuilder.Append(blogTitleItem);
                }
                else if (blogTitleItem == ' ')
                {
                    aliasBuilder.Append('-');
                }
            }

            return aliasBuilder.ToString();
        }
    }
}
