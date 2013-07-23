using System;
using deployd.AppStart;
using deployd.Features.FeatureSelection;

namespace deployd
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new ApplicationContext(args);
            var featureFactory = context.Kernel.GetService<ActiveFeatureFactory>();
            var chainOfCommands = featureFactory.BuildCommands();
            var returnCode = chainOfCommands.RunAll();

            Environment.Exit(returnCode);

        }
    }
}
