using System;
using System.IO;
using Utility.Strings;
using dotless.Core;

namespace Utility
{
    public static class LESS
    {
        public static void Save(string content, string outputFile, string pathLESS)
        {
            try
            {
                Directory.SetCurrentDirectory(Kandu.App.MapPath(pathLESS));
                var file = Kandu.App.MapPath(outputFile);
                var dir = file.Replace(file.GetFilename(), "");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var css = Less.Parse(content);
                File.WriteAllText(file, css);
                Directory.SetCurrentDirectory(Kandu.App.MapPath("/"));
            }
            catch (Exception ex)
            {
                throw new Kandu.ServiceErrorException("Error generating compiled LESS resource");
            }
        }
    }
}
