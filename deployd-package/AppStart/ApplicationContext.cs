using System;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using deployd_package.Features.MetadataDiscovery;
using log4net;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd_package.AppStart
{
    public class ApplicationContext
    {
        public IKernel Kernel { get; set; }

        public ApplicationContext()
        {
            Kernel = CreateKernel();
            log4net.Config.XmlConfigurator.Configure();

            var log = Kernel.Get<ILog>();
            log.Info("deployd-package");
            log.Info("version: " + GetType().Assembly.GetName().Version);
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ILog>().ToMethod(x => LogManager.GetLogger("default")).InSingletonScope();

            kernel.Bind(scanner => scanner.FromAssemblyContaining<IFileSystem>().Select(IsServiceType).BindDefaultInterfaces());

            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsHeuristic).BindDefaultInterfaces());

            return kernel;
        }

        private static bool IsServiceType(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == "I" + type.Name);
        }

        private static bool IsHeuristic(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == typeof(IMetadataDiscoveryHeuristic).Name);
        }
    }
}