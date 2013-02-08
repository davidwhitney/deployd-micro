using System;
using System.IO;
using System.Text;
using deployd.Extensibility.Configuration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.Help
{
    public class HelpCommand : IFeatureCommand
    {
        private readonly IInstanceConfiguration _config;

        public HelpCommand(IInstanceConfiguration config)
        {
            _config = config;
        }

        public void Execute()
        {
            if (!(_config is InstanceConfiguration))
            {
                return;
            }

            var cfg = (InstanceConfiguration)_config;

            var output = new StringBuilder();
            using (var textWriter = new StringWriter(output))
            {
                cfg.OptionSet.WriteOptionDescriptions(textWriter);
            }

            Console.WriteLine(output);
        }
    }
}
