using System.IO;
using System.IO.Compression;
using System.Text;

namespace Kandu.Common.Utility
{
    public static class Compression
    {
        public static void GzipCompress(string contents, string outfile)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(contents);
            MemoryStream ms = new MemoryStream(bytes);
            using (FileStream fs = File.Create(App.MapPath(outfile)))
            {
                using (var gz = new GZipStream(fs, CompressionMode.Compress))
                {
                    ms.CopyTo(gz);
                }
            }
        }
    }
}
