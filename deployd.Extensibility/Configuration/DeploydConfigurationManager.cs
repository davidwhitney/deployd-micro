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
                return new DeploydConfiguration {DefaultConfiguration = true};
            }

            var configFileContents = _fileSystem.File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<DeploydConfiguration>(configFileContents);
        }
    }
}