using System.IO.Abstractions;

namespace deployd.Infrastructure
{
    public static class DirectoryExtensions
    {
        public static void EnsureDirectoryExists(this IFileSystem fs, string fullPath)
        {
            if (!fs.Directory.Exists(fullPath))
            {
                fs.Directory.CreateDirectory(fullPath);
            }
        }
    }
}