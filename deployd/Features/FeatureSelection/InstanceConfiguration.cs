using System.Collections.Generic;
using NDesk.Options;
using deployd.Features.AppExtraction;
using deployd.Features.AppLocating;

namespace deployd.Features.FeatureSelection
{
    public class InstanceConfiguration
    {
        public string AppName { get; set; }
        public bool Install { get; set; }
        public bool Help { get; set; }
        public bool Verbose { get; set; }
        
        public List<string> ExtraParams { get; set; }
        public OptionSet OptionSet { get; set; }

        public PackageLocation<object> PackageLocation { get; set; }

        public ApplicationMap ApplicationMap { get; set; }
    }
}