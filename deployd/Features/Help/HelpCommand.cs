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
        private readonly TextWriter _output;

        public HelpCommand(IInstanceConfiguration config, ILog log, TextWriter output)
        {
            _config = config;
            _log = log;
            _output = output;
        }

        public void Execute()
        {
            _config.OptionSet.WriteOptionDescriptions(_output);

            _output.WriteLine();
            _output.WriteLine("Configuration options");
            _output.WriteLine();
            _output.WriteLine("Usage: deployd --config [option]=[value]");
            _output.WriteLine("Sets a configuration option to the provided value");
            _output.WriteLine();
            _output.WriteLine("Usage: deployd --config [option]");
            _output.WriteLine("Displays the current value of the configuration option");
            var properties = typeof(DeploydConfiguration).GetProperties();
            _output.WriteLine("Configurable options:");
            foreach (var p in properties)
            {
                _output.WriteLine(p.Name);
            }
            _output.Flush();
        }
    }
}
