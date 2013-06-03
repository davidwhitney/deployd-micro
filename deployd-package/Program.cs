using System;
using System.IO;
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

            if (!Directory.Exists(cfg.OutputDirectory))
            {
                Directory.CreateDirectory(cfg.OutputDirectory);
            }

            var packager = context.Kernel.Get<Packager>();
            packager.Package(cfg.SourceDirectory, cfg.OutputDirectory, cfg.TargetVersion);
        }     
    }
}
