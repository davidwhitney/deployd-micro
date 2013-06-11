using System;
using System.IO;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using NuGet;
using deployd.Extensibility.Configuration;
using deployd.Features;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation.HookExecution;
using deployd.Features.AppLocating;
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
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsHookRunner).BindAllInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsApplicationConfigurator).BindAllInterfaces());

            kernel.Bind(scanner => scanner.FromAssemblyContaining<IFileSystem>().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromAssemblyContaining<IPackageRepositoryFactory>().Select(IsServiceType).BindDefaultInterfaces());

            kernel.Rebind<IApplication>().ToMethod(x => x.Kernel.GetService<ApplicationFactory>().GetCurrent()).InSingletonScope();
            kernel.Rebind<IApplicationMap>().ToMethod(x => x.Kernel.GetService<IInstanceConfiguration>().ApplicationMap).InSingletonScope();

            kernel.Rebind<IInstanceConfiguration>().ToMethod(x => x.Kernel.GetService<IArgumentParser>().Parse(_args)).InSingletonScope();
            kernel.Bind<InstanceConfiguration>().ToMethod(x => x.Kernel.GetService<IInstanceConfiguration>() as InstanceConfiguration);
            kernel.Bind<DeploydConfiguration>().ToMethod(x => x.Kernel.GetService<DeploydConfigurationManager>().LoadConfig()).InSingletonScope();

            kernel.Bind<ILog>().ToMethod(x => LogManager.GetLogger("default")).InSingletonScope();
            kernel.Bind<Stream>().ToMethod(x => System.Console.OpenStandardOutput());

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

        private static bool IsApplicationConfigurator(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == typeof(IApplicationConfigurator).Name);
        }

        private static bool IsHookRunner(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == typeof(IHookRunner).Name);
        }
    }
}
