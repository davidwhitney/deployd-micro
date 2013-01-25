using System.IO;
using System.IO.Abstractions;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppInstallation
{
    public class AppInstallationCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;

        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppInstallationCommand(IFileSystem fs, Configuration configuration)
        {
            _fs = fs;
            Configuration = configuration;
        }

        public void Execute()
        {
        }
    }
}
