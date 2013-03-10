using System;
using Ninject;
using deployd_package.AppStart;
using deployd_package.Features.Configuration;

namespace deployd_package
{
    class Program
    {
        private static void Main(string[] args)
        {
            var context = new ApplicationContext();
            var cfg = new Configuration(args);

            if (cfg.ShowHelp)
            { 
                cfg.Options.WriteOptionDescriptions(Console.Out);
            }

            var packager = context.Kernel.Get<Packager>();
            packager.Package(cfg.SourceDirectory, cfg.OutputDirectory);
        }     
    }
}
