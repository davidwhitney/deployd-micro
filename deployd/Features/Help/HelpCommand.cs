using System;
using System.IO;
using System.Text;
using deployd.Extensibility.Configuration;

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
            var output = new StringBuilder();
            using (var textWriter = new StringWriter(output))
            {
                _config.OptionSet.WriteOptionDescriptions(textWriter);
            }

            Console.WriteLine(output);
        }
    }
}
