using System.Collections.Generic;
using NDesk.Options;
using deployd.Extensibility.Configuration;

namespace deployd.Features.FeatureSelection
{
    public class InstanceConfiguration : IInstanceConfiguration
    {
        public string AppName { get; set; }
        public bool Install { get; set; }
        public bool Help { get; set; }
        public bool Verbose { get; set; }
        
        public List<string> ExtraParams { get; set; }
        public OptionSet OptionSet { get; set; }

        public PackageLocation<object> PackageLocation { get; set; }

        public IApplicationMap ApplicationMap { get; set; }

        public InstanceConfiguration()
        {
            ApplicationMap = new ApplicationMap(string.Empty, string.Empty);
        }
    }
}