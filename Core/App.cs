using System.Collections.Generic;
using System.IO;

namespace Kandu
{
    public enum Environment
    {
        development = 0,
        staging = 1,
        production = 2
    }

    public static class App
    {
        public static Environment Environment { get; set; } = Environment.development;
        public static string Host { get; set; }
        public static bool IsDocker { get; set; }
        public static Dictionary<string, string> Languages { get; set; }
        public static string[] ServicePaths { get; set; }

        private static string _rootPath { get; set; }

        public static string RootPath
        {
            get
            {
                if (string.IsNullOrEmpty(_rootPath))
                {
                    _rootPath = Path.GetFullPath(".").Replace("\\", "/");
                }
                return _rootPath;
            }
        }

        public static string MapPath(string path = "")
        {
            path = path.Replace("\\", "/");
            if (path.Substring(0, 1) == "/") { path = path.Substring(1); }
            if (IsDocker)
            {
                return Path.Combine(RootPath, path);
            }
            else
            {
                return Path.Combine(RootPath.Replace("/", "\\"), path.Replace("/", "\\"));
            }
        }
    }
}
