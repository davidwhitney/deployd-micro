using System.IO;
using System.IO.Abstractions;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppExtractionCommand(IFileSystem fs, Configuration configuration)
        {
            _fs = fs;
            Configuration = configuration;

            if (!_fs.Directory.Exists(configuration.InstallRoot))
            {
                _fs.Directory.CreateDirectory(configuration.InstallRoot);
            }
        }

        public void Execute()
        {
            if (InstanceConfiguration.AppInstallationLocation == null)
            {
                return;
            }

            if (!_fs.Directory.Exists(InstanceConfiguration.AppName))
            {
                _fs.Directory.CreateDirectory(Path.Combine(Configuration.InstallRoot, InstanceConfiguration.AppName));
            }
            
        }
    }
}
