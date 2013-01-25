using System;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using NuGet;
using deployd.Features.AppExtraction;
using deployd.Features.AppLocating;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using log4net;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.AppStart
{
    public class ApplicationContext
    {
        private readonly string[] _args;
        public IKernel Kernel { get; set; }

        public ApplicationContext(string[] args)
        {
            _args = args;
            Kernel = CreateKernel();

            log4net.Config.XmlConfigurator.Configure();
            var log = Kernel.GetService<ILog>();
            log.Info("deployd-mini");
            log.Info("version: " + GetType().Assembly.GetName().Version);
        }

        private IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsInstallationLocator).BindAllInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsPackageExtractor).BindAllInterfaces());

            kernel.Bind(scanner => scanner.FromAssemblyContaining<IFileSystem>().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromAssemblyContaining<IPackageRepositoryFactory>().Select(IsServiceType).BindDefaultInterfaces());

            kernel.Bind<InstanceConfiguration>().ToMethod(x => kernel.GetService<IArgumentParser>().Parse(_args)).InSingletonScope();
            kernel.Bind<DeploydConfiguration>().ToMethod(x => kernel.GetService<DeploydConfigurationManager>().LoadConfig()).InSingletonScope();

            kernel.Bind<ILog>().ToMethod(x => LogManager.GetLogger("default")).InSingletonScope();

            return kernel;
        }

        private static bool IsServiceType(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == "I" + type.Name);
        }

        private static bool IsInstallationLocator(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == typeof(IAppInstallationLocator).Name);
        }

        private static bool IsPackageExtractor(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == typeof(IPackageExtractor).Name);
        }
    }
}
