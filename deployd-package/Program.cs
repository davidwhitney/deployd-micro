using System;
using System.IO.Abstractions;
using NuGet;

namespace deployd_package
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No path supplied.");
            }

            var directoryInspector = new PackageFileLocator(new FileSystem(), args[0]);

            var package = new PackageBuilder();
            foreach (var file in directoryInspector.IncludedFiles())
            {
                package.Files.Add(file);
            }

        }
    }
}
