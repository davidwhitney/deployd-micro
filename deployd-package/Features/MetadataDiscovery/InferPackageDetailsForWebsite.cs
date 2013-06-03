using System.IO.Abstractions;
using log4net;

namespace deployd_package.Features.MetadataDiscovery
{
    public class InferPackageDetailsForWebsite : IMetadataDiscoveryHeuristic
    {
        private readonly IFileSystem _fs;
        private readonly IPackageDetailsFromAssemblyMapper _fromAssemblyMapper;
        private readonly ILog _log;

        public InferPackageDetailsForWebsite(IFileSystem fs, IPackageDetailsFromAssemblyMapper fromAssemblyMapper, ILog log)
        {
            _fs = fs;
            _fromAssemblyMapper = fromAssemblyMapper;
            _log = log;
        }

        public void DiscoverMetadataProperties(PackageMetadata discoveredMetadata, string discoveryRoot)
        {
            // don't bother if this isn't a website
            if (!_fs.File.Exists(_fs.Path.Combine(discoveryRoot, "web.config"))) return;

            // the name of the folder that contains the top-most web.config file should be the name of the website
            var webAppName = _fs.DirectoryInfo.FromDirectoryName(discoveryRoot).Name;

            // assuming this is a pre-compiled website there should be an assembly with this name in the bin folder
            string webAppAssemblyPath = _fs.Path.Combine(discoveryRoot, string.Format("bin\\{0}.dll", webAppName));
            if (_fs.File.Exists(webAppAssemblyPath))
            {
                _log.DebugFormat("Inferring package metadata for web application {0}", webAppAssemblyPath);
                _fromAssemblyMapper.MapAssemblyInfoToPackage(webAppAssemblyPath, discoveredMetadata);
            }
        }
    }
}