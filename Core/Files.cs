using Utility.Strings;
namespace Kandu.Core
{
    public static class Files
    {
        public enum FileType
        {
            Unknown = -1,
            Image = 0,
            Document = 1,
            Compressed = 2,
            Video = 3,
            Audio = 4
        }

        public static FileType GetFileType(string filename)
        {
            var ext = filename.GetFileExtension();
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                    return FileType.Image;
                case "doc":
                case "docx":
                case "rtf":
                case "pdf":
                case "txt":
                case "csv":
                case "xls":
                case "xlsx":
                    return FileType.Document;
                case "zip":
                case "rar":
                case "7z":
                    return FileType.Compressed;
                    break;
                case "mp4":
                case "flv":
                case "ogg":
                case "avi":
                case "divx":
                case "xvid":
                case "mkv":
                    return FileType.Video;
                case "mp3":
                case "flac":
                case "wav":
                case "aac":
                    return FileType.Audio;
                default:
                    return FileType.Unknown;
            }
        }
    }
}
