using deployd.AppStart;
using deployd.Features.FeatureSelection;

namespace deployd
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new ApplicationContext();
            var featureFactory = context.Kernel.GetService<ActiveFeatureFactory>();
            var chainOfCommands = featureFactory.BuildCommandsFor(args);

            foreach (var command in chainOfCommands)
            {
                command.Execute();
            }
        }
    }
}
