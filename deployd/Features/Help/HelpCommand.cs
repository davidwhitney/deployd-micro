using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using deployd.Extensibility.Configuration;
using log4net;

namespace deployd.Features.Help
{
    public class HelpCommand : IFeatureCommand
    {
        private readonly IInstanceConfiguration _config;
        private readonly ILog _log;
        private readonly Stream _outputStream;

        public HelpCommand(IInstanceConfiguration config, ILog log, Stream outputStream)
        {
            _config = config;
            _log = log;
            _outputStream = outputStream;
        }

        public void Execute()
        {
            var output = new StringBuilder();
            using (var textWriter = new StringWriter(output))
            {
                _config.OptionSet.WriteOptionDescriptions(textWriter);
            }

            output.AppendLine();
            output.AppendLine("Configuration options");
            output.AppendLine();
            output.AppendLine("Usage: deployd --config [option]=[value]");
            output.AppendLine("Sets a configuration option to the provided value");
            output.AppendLine();
            output.AppendLine("Usage: deployd --config [option]");
            output.AppendLine("Displays the current value of the configuration option");
            var properties = typeof(DeploydConfiguration).GetProperties();
            output.AppendLine("Configurable options:");
            foreach (var p in properties)
            {
                output.AppendLine(p.Name);
            }

            using (var writer = new StreamWriter(_outputStream))
            {
                writer.Write(output);
                writer.Flush();
            }
        }
    }
}
