using System;
using System.IO;
using System.IO.Abstractions;
using Newtonsoft.Json;

namespace deployd.Extensibility.Configuration
{
    public class DeploydConfigurationManager
    {
        private readonly IFileSystem _fileSystem;
        private readonly IApplicationFolderLocator _applicationFolderLocator;
        const string fileName = "config.json";
        private readonly string _filePath = "";

        public DeploydConfigurationManager(IFileSystem fileSystem, IApplicationFolderLocator applicationFolderLocator)
        {
            _fileSystem = fileSystem;
            _applicationFolderLocator = applicationFolderLocator;
            _filePath = _fileSystem.Path.Combine(_applicationFolderLocator.ApplicationFolder, fileName);
        }

        public DeploydConfiguration LoadConfig()
        {
            if (!_fileSystem.File.Exists(_filePath))
            {
                var configuration = new DeploydConfiguration();
                using (var file = _fileSystem.File.Open(_filePath, FileMode.Create, FileAccess.Write))
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

            var configFileContents = _fileSystem.File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<DeploydConfiguration>(configFileContents);
        }

        public void SaveConfig(DeploydConfiguration deploydConfiguration)
        {
            using (var file = _fileSystem.File.Open(_filePath, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new StreamWriter(file))
            {
                streamWriter.Write(JsonConvert.SerializeObject(deploydConfiguration));
                streamWriter.Flush();
                streamWriter.Close();
            }
        }
    }
}