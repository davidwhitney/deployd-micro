using System;
using System.Linq;
using NDesk.Options;
using deployd.Extensibility;

namespace deployd_package.Features.Configuration
{
    public class Configuration
    {
        public OptionSet Options { get; private set; }

        public string SourceDirectory { get; private set; }
        public string OutputDirectory { get; private set; }
        public bool ShowHelp { get; private set; }
        public Version TargetVersion { get; set; }

        public Configuration(string[] args)
        {
            Options = new OptionSet
                {
                    {"source=", v => SourceDirectory = v},
                    {"target=", v => OutputDirectory = v},
                    {"forceversion|fv=", v=>
                        {
                            Version version;
                            Version.TryParse(v, out version);
                            TargetVersion = version;
                        }
                    },
                    {"help|h|?", v => ShowHelp = v != null},
                };
            Options.Parse(args);

            if (args == null || !args.Any())
            {
                ShowHelp = true;
            }

            if (string.IsNullOrWhiteSpace(SourceDirectory))
            {
                SourceDirectory = "~/".ToAbsolutePath();
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                OutputDirectory = "~/".ToAbsolutePath();
            }
        }
    }
}
