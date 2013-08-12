using System;
using System.Collections.Generic;
using System.Linq;
using NDesk.Options;
using NuGet;

namespace deployd.Features.FeatureSelection
{
    public class ArgumentParser : IArgumentParser
    {
        public InstanceConfiguration Parse(IList<string> args)
        {
            var cfg = new InstanceConfiguration
                {
                    AppName = string.Empty,
                    Help = false,
                    Verbose = false,
                    ExtraParams = new List<string>()
                };

            var p = new OptionSet
                {
                    {"a|app=", v => cfg.AppName = v},
                    {"i|install", v => cfg.Install = v != null},
                    {"u|update", v => cfg.Update = v != null},
                    {"p|prep", v => cfg.Prep = v != null},
                    {"v|version", version =>
                        {
                            if (string.IsNullOrWhiteSpace(version))
                            {
                                return;
                            }
                            SemanticVersion semanticVersion;
                            if (!SemanticVersion.TryParse(version, out semanticVersion))
                            {
                                throw new ArgumentException("Version must be a valid version format", "version");
                            }
                            cfg.ApplicationVersion = semanticVersion;
                        }},
                    {"e|environment=", v => cfg.Environment = v},
                    {"verbose", v => cfg.Verbose = v != null},
                    {"s|state", v => cfg.ShowState = v != null},
                    {"f|force", v => cfg.ForceDownload = v != null},
                    {"c|config=", v => cfg.SetConfigurationValue = v},
                    {"h|?|help", v => cfg.Help = v != null},
                };
            cfg.OptionSet = p;

            if (args == null || !args.Any())
            {
                cfg.Help = true;
                return cfg;
            }

            cfg.ExtraParams = p.Parse(args);
            cfg.AppName = cfg.AppName.Trim('"', '\'');
            return cfg;
        }
    }
}
