using System;
using System.IO;
using System.IO.Abstractions;
using NuGet;

namespace deployd_package
{
    class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No path supplied.");
            }

            var directoryInspector = new PackageFileLocator(new FileSystem(), args[0]);

            // need to fill in order discover all these bits
            var package = new PackageBuilder
                {
                    Id = "temporary-id",
                    Version = new SemanticVersion(1, 0, 0, 0),
                    Description = "desc"
                };
            package.Authors.Add("Author");

            foreach (var file in directoryInspector.IncludedFiles())
            {
                package.Files.Add(file);
            }

            var packageOutputFilename = string.Format("{0}-{1}.nupkg", package.Id, package.Version);
            using (var fs = new FileStream(packageOutputFilename, FileMode.Create))
            {
                package.Save(fs);
                fs.Close();
            }
        }
    }
}
