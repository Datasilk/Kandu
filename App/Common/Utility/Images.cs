using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Saber.Common.Utility
{
    public struct ImageInfo
    {
        public string path;
        public string filename;
        public int width;
        public int height;
        public Image<Rgba32> bitmap;
    }

    public class Images
    {
        public ImageInfo Load(string path, string filename)
        {
            ImageInfo newImg = new ImageInfo();
            using (var fs = File.OpenRead(Server.MapPath(path + filename)))
            {
                var image = Image.Load<Rgba32>(fs);
                newImg.bitmap = image;
                newImg.filename = filename;
                newImg.path = path;
                newImg.width = image.Width;
                newImg.height = image.Height;
            }
            return newImg;
        }

        public void Shrink(string filename, string outfile, int width)
        {
            using (var fs = File.OpenRead(Server.MapPath(filename)))
            {
                var image = Image.Load(fs);

                if (image.Width > width)
                {
                    image.Mutate(img => img.Resize(new ResizeOptions()
                    {
                        Size = new Size(width, 0)
                    }));
                }
                image.Save(Server.MapPath(outfile));
                fs.Dispose();
            }
        }
    }
}
