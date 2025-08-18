using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;

namespace BlogApp.Infrastructure
{
    public class ImageConvertor
    {
        public void ConvertToWebpFormat(string currentImagePath, string targetImagePath)
        {
            using (Image image = Image.Load(currentImagePath))
            {
                image.Save(targetImagePath, new WebpEncoder());
            }
        }
    }
}
