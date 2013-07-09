using System;
using System.IO;
using System.Linq;
using Ninject;
using NuGet;
using deployd.Extensibility.Configuration;
using deployd.Features;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppInstallation.HookExecution;
using deployd.Features.AppInstallation.Hooks;
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

            /*
             * convention based bindings
             * Requires Ninject.Conventions.Extensions which depends on .Net 4.5
             * We have to upgrade servers to 2008+ in order to use this
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsInstallationLocator).BindAllInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsPackageExtractor).BindAllInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsHookRunner).BindAllInterfaces());
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsApplicationConfigurator).BindAllInterfaces());

            kernel.Bind(scanner => scanner.FromAssemblyContaining<IFileSystem>().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromAssemblyContaining<IPackageRepositoryFactory>().Select(IsServiceType).BindDefaultInterfaces());
             */
            BindEverything(kernel);

            kernel.Bind<IApplicationFactory>().To<ApplicationFactory>();
            kernel.Bind<IInstallationRoot>().To<InstallationRoot>();
            kernel.Bind<IInstallHookExecutor>().To<InstallHookExecutor>();
            kernel.Bind<IHookFinder>().To<HookFinder>();
            kernel.Rebind<IApplication>().ToMethod(x => x.Kernel.GetService<ApplicationFactory>().GetCurrent()).InSingletonScope();
            kernel.Rebind<IApplicationMap>().ToMethod(x => x.Kernel.GetService<IInstanceConfiguration>().ApplicationMap).InSingletonScope();

            kernel.Rebind<IInstanceConfiguration>().ToMethod(x => x.Kernel.GetService<IArgumentParser>().Parse(_args)).InSingletonScope();
            kernel.Bind<InstanceConfiguration>().ToMethod(x => x.Kernel.GetService<IInstanceConfiguration>() as InstanceConfiguration);
            kernel.Bind<DeploydConfiguration>().ToMethod(x => x.Kernel.GetService<DeploydConfigurationManager>().LoadConfig()).InSingletonScope();

            kernel.Bind<ILog>().ToMethod(x => LogManager.GetLogger("default")).InSingletonScope();
            kernel.Bind<Stream>().ToMethod(x=>System.Console.OpenStandardOutput());

            return kernel;
        }

        private void BindEverything(StandardKernel kernel)
        {
            kernel.Bind<IArgumentParser>().To<ArgumentParser>();

            kernel.Bind<ILoggingConfiguration>().To<LoggingConfiguration>();

            kernel.Bind<IInstallationPadLock>().To<InstallationPadLock>();

            kernel.Bind<IFileSystem>().To<System.IO.Abstractions.FileSystem>();
            kernel.Bind<IPackageRepositoryFactory>().To<PackageRepositoryFactory>();

            kernel.Bind<IListLatestVersionsOfPackagesQuery>().To<ListLatestVersionOfPackagesQuery>();
            kernel.Bind<IGetLatestNuGetPackageByNameQuery>().To<GetLatestNuGetPackageByNameQuery>();
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
