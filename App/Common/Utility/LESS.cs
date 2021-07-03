using System;
using System.IO;
using Utility.Strings;
using dotless.Core;

namespace Kandu.Common.Utility
{
    public static class LESS
    {
        public static void Save(string content, string outputFile, string pathLESS)
        {
            try
            {
                Directory.SetCurrentDirectory(App.MapPath(pathLESS));
                var file = App.MapPath(outputFile);
                var dir = file.Replace(file.GetFilename(), "");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var css = Less.Parse(content);
                File.WriteAllText(file, css);
                Directory.SetCurrentDirectory(App.MapPath("/"));
            }
            catch (Exception ex)
            {
                throw new ServiceErrorException("Error generating compiled LESS resource");
            }
        }
    }
}
