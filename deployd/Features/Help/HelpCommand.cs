using System.IO;
using System.Text;
using deployd.Extensibility.Configuration;
using log4net;

namespace deployd.Features.Help
{
    public class HelpCommand : IFeatureCommand
    {
        private readonly IInstanceConfiguration _config;
        private readonly ILog _log;

        public HelpCommand(IInstanceConfiguration config, ILog log)
        {
            _config = config;
            _log = log;
        }

        public void Execute()
        {
            var output = new StringBuilder();
            using (var textWriter = new StringWriter(output))
            {
                _config.OptionSet.WriteOptionDescriptions(textWriter);
            }

            _log.Info(output.ToString());
        }
    }
}
