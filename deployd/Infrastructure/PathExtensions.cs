using System;
using System.IO;

namespace deployd.Infrastructure
{
    public static class PathExtensions
    {
        public static string GetAppInstallDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string ToAbsolutePath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            if ((path[1] == ':' && path[2] == '\\'))
            {
                return path;
            }

            if (!path.StartsWith("~/")
                && !path.StartsWith("~\\")
                && Path.GetFileName(path) != path
                && !path.Contains("/"))
            {
                return path;
            }

            var thisClassesPyhsicalPath = GetAppInstallDirectory();
            var relativePath = path.Replace("~\\", "").Replace("~/", "").Replace("/", "\\");
            return Path.Combine(thisClassesPyhsicalPath, relativePath);
        }
    }
}
