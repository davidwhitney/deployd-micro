using System.IO.Abstractions;

namespace deployd.Infrastructure
{
    public static class DirectoryExtensions
    {
        public static void EnsureDirectoryExists(this IFileSystem fs, string path)
        {
            if (!fs.Directory.Exists(path))
            {
                fs.Directory.CreateDirectory(path);
            }
        }
    }
}