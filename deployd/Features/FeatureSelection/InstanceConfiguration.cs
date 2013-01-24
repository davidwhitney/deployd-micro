using System.Collections.Generic;

namespace deployd.Features.FeatureSelection
{
    public class InstanceConfiguration
    {
        public string PackageName { get; set; }
        public bool Help { get; set; }
        public bool Verbose { get; set; }
        public List<string> ExtraParams { get; set; }
    }
}