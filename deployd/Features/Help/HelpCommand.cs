using System;
using System.IO;
using System.Text;
using deployd.Extensibility.Configuration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.Help
{
    public class HelpCommand : IFeatureCommand
    {
        public DeploydConfiguration DeploydConfiguration { get; set; }
        public IInstanceConfiguration Config { get; set; }

        public void Execute()
        {
            if (!(Config is InstanceConfiguration))
            {
                return;
            }

            var cfg = (InstanceConfiguration)Config;

            var output = new StringBuilder();
            using (var textWriter = new StringWriter(output))
            {
                cfg.OptionSet.WriteOptionDescriptions(textWriter);
            }

            Console.WriteLine(output);
        }
    }
}
