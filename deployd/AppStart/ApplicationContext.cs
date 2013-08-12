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
using deployd.Features.Environment;
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

            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

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
            kernel.Rebind<IApplication>().ToMethod(x => x.Kernel.GetService<ApplicationFactory>().GetCurrent()).InSingletonScope();
            kernel.Rebind<IApplicationMap>().ToMethod(x => x.Kernel.GetService<IInstanceConfiguration>().ApplicationMap).InSingletonScope();

            kernel.Bind<IPackageCache>().To<PackageCache>();
            /*kernel.Bind<IAppInstallationLocator>()
                .ToConstructor(a =>
                    new NuGetFeedAppInstallationLocator(AppDomain.CurrentDomain.BaseDirectory + "\\cache", a.Inject<IFileSystem>(), a.Inject<IGetLatestNuGetPackageByNameQuery>()));*/
            kernel.Bind<IAppInstallationLocator>().To<NuGetFeedAppInstallationLocator>();
            kernel.Bind<IAppInstallationLocator>()
                .ToConstructor(a=>new FileSystemAppInstallationLocator(a.Inject<DeploydConfiguration>(),a.Inject<IFileSystem>(),a.Inject<ILog>()));

            kernel.Bind<IPackageExtractor>().To<NuGetPackageExtractor>();
            kernel.Bind<IPackageExtractor>().To<ZipFilePackageExtractor>();

            kernel.Rebind<IInstanceConfiguration>().ToMethod(x => x.Kernel.GetService<IArgumentParser>().Parse(_args)).InSingletonScope();
            kernel.Bind<InstanceConfiguration>().ToMethod(x => x.Kernel.GetService<IInstanceConfiguration>() as InstanceConfiguration);
            kernel.Bind<DeploydConfiguration>().ToMethod(x => x.Kernel.GetService<DeploydConfigurationManager>().LoadConfig()).InSingletonScope();

            kernel.Bind<ILog>().ToMethod(x => LogManager.GetLogger("default")).InSingletonScope();
            kernel.Bind<TextWriter>().ToMethod(x => Console.Out);

            kernel.Bind<IHookFinder>().To<HookFinder>();
            kernel.Bind<IHookRunner>().To<PowershellRunner>();
            kernel.Bind<IHookRunner>().To<ClassHookRunner>();
            kernel.Bind<IHookRunner>().To<CommandLineRunner>();

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
            kernel.Bind<IGetNuGetPackageByNameAndVersionQuery>().To<GetNuGetPackageByNameAndVersionQuery>();
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
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == typeof(IEnvironmentApplier).Name);
        }

        private static bool IsHookRunner(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == typeof(IHookRunner).Name);
        }
    }
}
