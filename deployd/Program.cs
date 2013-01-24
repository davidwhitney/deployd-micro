using deployd.AppStart;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new ApplicationContext();
            var configManager = context.Kernel.GetService<ClientConfigurationManager>();
            var configuration = configManager.LoadConfig();

            var argParser = context.Kernel.GetService<ArgumentParser>();
            var instanceConfiguration = argParser.Parse(args);
        }
    }
}
