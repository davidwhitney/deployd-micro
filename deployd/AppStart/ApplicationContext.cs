using System;
using System.IO.Abstractions;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;

namespace deployd.AppStart
{
    public class ApplicationContext
    {
        public IKernel Kernel { get; set; }

        public ApplicationContext()
        {
            Kernel = CreateKernel();
        }

        private IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind(scanner => scanner.FromThisAssembly().Select(IsServiceType).BindDefaultInterfaces());
            kernel.Bind(scanner => scanner.FromAssemblyContaining<IFileSystem>().Select(IsServiceType).BindDefaultInterfaces());

            return kernel;
        }

        private static bool IsServiceType(Type type)
        {
            return type.IsClass && type.GetInterfaces().Any(intface => intface.Name == "I" + type.Name);
        }
    }
}
