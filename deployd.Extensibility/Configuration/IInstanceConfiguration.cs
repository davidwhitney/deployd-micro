
using System.Collections.Generic;
using NDesk.Options;

namespace deployd.Extensibility.Configuration
{
    public interface IInstanceConfiguration
    {
        string AppName { get; set; }
        bool Install { get; set; }
        bool Prep { get; set; }
        bool Help { get; set; }
        bool Verbose { get; set; }
        
        List<string> ExtraParams { get; set; }
        OptionSet OptionSet { get; set; }

        PackageLocation<object> PackageLocation { get; set; }

        IApplicationMap ApplicationMap { get; set; }
        string Environment { get; set; }
        bool ShowState { get; set; }
        bool Update { get; set; }
        bool ForceDownload { get; set; }
        string SetConfigurationValue { get; set; }
    }
}