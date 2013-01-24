using System.Collections.Generic;
using NDesk.Options;

namespace deployd.Features.FeatureSelection
{
    public class ArgumentParser
    {
        public InstanceConfiguration Parse(IEnumerable<string> args)
        {
            var cfg = new InstanceConfiguration
                {
                    PackageName = string.Empty,
                    Help = false,
                    Verbose = false,
                    ExtraParams = new List<string>()
                };

            var p = new OptionSet
                {
                    {"file=", v => cfg.PackageName = v},
                    {"v|verbose", v => cfg.Verbose = v != null},
                    {"h|?|help", v => cfg.Help = v != null},
                };

            cfg.ExtraParams = p.Parse(args);

            return cfg;
        }
    }
}
