using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using deployd.Features.AppInstallation;
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
        private readonly IInstanceConfiguration _instanceConfiguration;
        private readonly IListLatestVersionsOfPackagesQuery _query;
        private readonly IFileSystem _fs;
        private readonly IPackageSourceConfiguration _packageSourceConfiguration;
        private readonly IInstallationRoot _installationRoot;
        private ILog _logger = LogManager.GetLogger(typeof (ShowStateCommand));

        public ShowStateCommand(IApplication app,
                                TextWriter output, IEnumerable<IAppInstallationLocator> finders,
                                IInstanceConfiguration instanceConfiguration,
                                IListLatestVersionsOfPackagesQuery query,
                                System.IO.Abstractions.IFileSystem fs,
            IPackageSourceConfiguration packageSourceConfiguration,
            IInstallationRoot installationRoot)
        {
            _app = app;
            _output = output;
            _finders = finders;
            _instanceConfiguration = instanceConfiguration;
            _query = query;
            _fs = fs;
            _packageSourceConfiguration = packageSourceConfiguration;
            _installationRoot = installationRoot;
        }

        public void Execute()
        {
            _output.WriteLine();

            var installed = new Dictionary<IPackage, Application>();
            var notInstalled = new List<IPackage>();
            var allPackages = _query.GetLatestVersions(_packageSourceConfiguration.PackageSource);
            foreach (var sourcePackage in allPackages)
            {
                string installPath = _fs.Path.Combine(_installationRoot.Path, sourcePackage.Id);
                _logger.DebugFormat("Checking {0}", installPath);
                
                var appMap = new ApplicationMap(sourcePackage.Id, installPath);
                var application = new Application(appMap, _fs, _logger, _instanceConfiguration, new InstallationPadLock(appMap, _fs), _output);


                if (_fs.File.Exists(appMap.VersionFile))
                {
                    _logger.DebugFormat("{0} exists", installPath);
                    SemanticVersion installedVersion;
                    if (SemanticVersion.TryParse(_fs.File.ReadAllText(appMap.VersionFile), out installedVersion))
                    {
                        installed.Add(sourcePackage, application);

                        if (application.GetInstalledVersion() < sourcePackage.Version.Version)
                        {
                            notInstalled.Add(sourcePackage);
                        }
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
                var installedVersion = installed[package].GetInstalledVersion();
                _output.Write("{0} {1}{2}", package.Id,
                    installedVersion,
                    installedVersion < package.Version.Version ? "*" : "");

                if (installed[package].IsStaged)
                {
                    var stagedVersion = installed[package].GetStagedVersion();
                    if (stagedVersion != null && stagedVersion != installedVersion)
                    {
                        _output.Write("  (staged: {0})", stagedVersion);
                    }
                }
                _output.WriteLine();

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