using System.IO;
using System.IO.Abstractions;
using Newtonsoft.Json;

namespace deployd.Extensibility.Configuration
{
    public class DeploydConfigurationManager
    {
        private readonly IFileSystem _fileSystem;

        public DeploydConfigurationManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public DeploydConfiguration LoadConfig()
        {
            const string fileName = "config.json";

            if (!_fileSystem.File.Exists(fileName))
            {
                var configuration = new DeploydConfiguration
                    {
                        DefaultConfiguration = true
                    };
                using (var file = _fileSystem.File.Open(fileName, FileMode.Create, FileAccess.Write))
                using (var streamWriter = new StreamWriter(file))
                {
                    configuration.PackageType = PackageType.NuGet;
                    configuration.PackageSource = @"c:\path\to\packages";
                    configuration.InstallRoot = @"c:\where\to\install\packages\to";
                    streamWriter.Write(JsonConvert.SerializeObject(configuration));
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            var configFileContents = _fileSystem.File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<DeploydConfiguration>(configFileContents);
        }
    }
}