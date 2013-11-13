using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;
using deployd.Extensibility.Configuration;
using deployd.Features.AppLocating;
using log4net;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.Features.ShowState
{
    public class ShowStateCommand : IFeatureCommand
    {
        private readonly IApplication _app;
        private readonly TextWriter _output;
        private readonly IEnumerable<IAppInstallationLocator> _finders;
        private readonly IInstanceConfiguration _config;
        private readonly IListLatestVersionsOfPackagesQuery _query;
        private readonly IFileSystem _fs;
        private readonly IPackageSourceConfiguration _packageSourceConfiguration;
        private readonly IInstallationRoot _installationRoot;
        private ILog _logger = LogManager.GetLogger(typeof (ShowStateCommand));

        public ShowStateCommand(IApplication app,
                                TextWriter output, IEnumerable<IAppInstallationLocator> finders,
                                IInstanceConfiguration config,
                                IListLatestVersionsOfPackagesQuery query,
                                System.IO.Abstractions.IFileSystem fs,
            IPackageSourceConfiguration packageSourceConfiguration,
            IInstallationRoot installationRoot)
        {
            _app = app;
            _output = output;
            _finders = finders;
            _config = config;
            _query = query;
            _fs = fs;
            _packageSourceConfiguration = packageSourceConfiguration;
            _installationRoot = installationRoot;
        }

        public void Execute()
        {
            _output.WriteLine();

            Dictionary<IPackage, bool> installed = new Dictionary<IPackage, bool>();
            List<IPackage> notInstalled = new List<IPackage>();
            var allPackages = _query.GetLatestVersions(_packageSourceConfiguration.PackageSource);
            foreach (var sourcePackage in allPackages)
            {
                string installPath = _fs.Path.Combine(_installationRoot.Path, sourcePackage.Id);
                _logger.DebugFormat("Checking {0}", installPath);
                var appMap = new ApplicationMap(sourcePackage.Id, installPath);
                if (_fs.File.Exists(appMap.VersionFile))
                {
                    _logger.DebugFormat("{0} exists", installPath);
                    SemanticVersion installedVersion;
                    if (SemanticVersion.TryParse(_fs.File.ReadAllText(appMap.VersionFile), out installedVersion))
                    {
                        installed.Add(sourcePackage, installedVersion < sourcePackage.Version);
                    }
                    else
                    {
                        notInstalled.Add(sourcePackage);
                    }
                }
                else
                {
                    _logger.DebugFormat("{0} not found", installPath);
                    notInstalled.Add(sourcePackage);
                }
            }

            _output.WriteLine("Installed packages: (* = updates are available)");
            foreach (var package in installed.Keys)
            {
                _output.WriteLine("{0}{1}", package.Id, installed[package] ? "*" : "");
            }

            _output.WriteLine();
            _output.WriteLine("Available packages:");
            foreach (var package in notInstalled)
            {
                _output.WriteLine("{0} {1}", package.Id, package.Version);
            }
        }
    }
}