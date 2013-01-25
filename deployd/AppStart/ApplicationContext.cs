using System;
using System.IO.Abstractions;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

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
        }

        private IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromAssemblyContaining<IFileSystem>().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind<InstanceConfiguration>().ToMethod(x => kernel.GetService<IArgumentParser>().Parse(_args)).InSingletonScope();
            kernel.Bind<Configuration>().ToMethod(x => kernel.GetService<ClientConfigurationManager>().LoadConfig()).InSingletonScope();

            return kernel;
        }

        private static bool IsServiceType(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == "I" + type.Name);
        }
    }
}
