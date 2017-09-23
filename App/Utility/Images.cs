using System;
using System.IO;
//using Nine.Imaging;
//using Nine.Imaging.Filtering;
//using Nine.Imaging.Encoding;

namespace Kandu.Utility
{
    public struct structImage
    {
        public string path;
        public string filename;
        public int width;
        public int height;
        //public Image bitmap;
    }
        

    public class Images
    {
        private Core S;

        public Images(Core KanduCore)
        {
            S = KanduCore;
        }

        /*
        public structImage Load(string path, string filename)
        {
            Image image = new Image(File.OpenRead(S.Server.MapPath(path + filename)));
            structImage newImg = new structImage();
            newImg.bitmap = image;
            newImg.filename = filename;
            newImg.path = path;
            newImg.width = image.PixelWidth;
            newImg.height = image.PixelHeight;
            return newImg;
        }

        public void Shrink(string filename, string outfile, int width)
        {
            FileStream fs = File.OpenRead(S.Server.MapPath(filename));
            Image image = new Image(fs);
            
            if (image.PixelWidth > width)
            {
                image = image.Resize(width);
            }
            Save(outfile, image);
            fs.Dispose();
        }

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
