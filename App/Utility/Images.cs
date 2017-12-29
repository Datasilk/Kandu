using System.IO;
using ImageSharp;

namespace Utility
{
    public struct structImage
    {
        public string path;
        public string filename;
        public int width;
        public int height;
        public Image<Rgba32> bitmap;
    }
        

    public class Images
    {
        private Core S;

        public Images(Core LegendaryCore)
        {
            S = LegendaryCore;
        }

        
        public structImage Load(string path, string filename)
        {
            structImage newImg = new structImage();
            using (var fs = File.OpenRead(S.Server.MapPath(path + filename)))
            {
                var image = Image.Load(fs);
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
            using (var fs = File.OpenRead(filename))
            {
                var image = Image.Load(fs);

                if (image.Width > width)
                {
                    image = image.Resize(new ImageSharp.Processing.ResizeOptions()
                    {
                        Size = new SixLabors.Primitives.Size(width, 0)
                    });
                }
                image.Save(outfile);
                fs.Dispose();
            }
        }
        /*
        public void Save(string filename, Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                string ext = S.Util.Str.getFileExtension(filename);
                switch (ext)
                {
                    case "jpg":
                    case "jpeg":
                        JpegEncoder jpgencoder = new JpegEncoder();
                        image.Save(ms, jpgencoder);
                        break;

                    case "png":
                        PngEncoder pngencoder = new PngEncoder();
                        image.Save(ms, pngencoder);
                        break;

                }
                //save to disk
                using (FileStream fs = new FileStream(S.Server.MapPath(filename), FileMode.Create))
                {
                    ms.Position = 0;
                    ms.WriteTo(fs);
                    fs.Dispose();
                }
                ms.Dispose();
            }
        }

        public void GeneratePhotos(string path, string name)
        {

            //Shrink(image, path + name, 4096);
            try {
                Shrink(path + name, path + "xl" + name, 1920);
                Shrink(path + name, path + "lg" + name, 800);
                Shrink(path + name, path + "med" + name, 400);
                Shrink(path + name, path + "sm" + name, 200);
                Shrink(path + name, path + "tiny" + name, 100);
                Shrink(path + name, path + "icon" + name, 50);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
        */
    }
}
