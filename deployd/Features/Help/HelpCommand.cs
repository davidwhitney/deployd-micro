using System;
using System.IO;
using System.Text;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.Help
{
    public class HelpCommand : IFeatureCommand
    {
        public DeploydConfiguration DeploydConfiguration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public void Execute()
        {
            var output = new StringBuilder();
            using (var textWriter = new StringWriter(output))
            {
                InstanceConfiguration.OptionSet.WriteOptionDescriptions(textWriter);
            }

            Console.WriteLine(output);
        }
    }
}
