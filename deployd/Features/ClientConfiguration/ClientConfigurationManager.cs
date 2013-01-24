using System.IO.Abstractions;
using Newtonsoft.Json;

namespace deployd.Features.ClientConfiguration
{
    public class ClientConfigurationManager
    {
        private readonly IFileSystem _fileSystem;

        public ClientConfigurationManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Configuration LoadConfig()
        {
            const string fileName = "config.json";

            if (!_fileSystem.File.Exists(fileName))
            {
                return new Configuration();
            }

            var configFileContents = _fileSystem.File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<Configuration>(configFileContents);
        }
    }
}