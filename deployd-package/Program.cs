using System;
using Ninject;
using deployd_package.AppStart;

namespace deployd_package
{
    class Program
    {
        private static void Main(string[] args)
        {
            var context = new ApplicationContext();

            if (args.Length < 1)
            { 
                Console.WriteLine("No path supplied.");
            }

            var packageSourceRoot = args[0];
            var packager = context.Kernel.Get<Packager>();
            packager.Package(packageSourceRoot, string.Empty);
        }     
    }
}
