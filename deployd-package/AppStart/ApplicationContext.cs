using System;
using System.IO.Abstractions;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using deployd_package.Features.MetadataDiscovery;
using log4net;

namespace deployd_package.AppStart
{
    public class ApplicationContext
    {
        public IKernel Start()
        {
            var kernel = CreateKernel();
            log4net.Config.XmlConfigurator.Configure();

            var log = kernel.Get<ILog>();
            log.Info("deployd-package");
            log.Info("version: " + typeof(Program).Assembly.GetName().Version);

            return kernel;
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsHeuristic).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromAssemblyContaining<IFileSystem>().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind<ILog>().ToMethod(x => LogManager.GetLogger("default")).InSingletonScope();

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