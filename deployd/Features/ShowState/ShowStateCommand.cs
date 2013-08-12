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
        private readonly Stream _outputStream;
        private readonly IEnumerable<IAppInstallationLocator> _finders;
        private readonly IInstanceConfiguration _config;
        private readonly IListLatestVersionsOfPackagesQuery _query;
        private readonly DeploydConfiguration _deployd;
        private readonly IFileSystem _fs;
        private ILog _logger = LogManager.GetLogger(typeof (ShowStateCommand));

        public ShowStateCommand(IApplication app,
            Stream outputStream, IEnumerable<IAppInstallationLocator> finders, IInstanceConfiguration config,
            IListLatestVersionsOfPackagesQuery query, DeploydConfiguration deployd, System.IO.Abstractions.IFileSystem fs)
        {
            _app = app;
            _outputStream = outputStream;
            _finders = finders;
            _config = config;
            _query = query;
            _deployd = deployd;
            _fs = fs;
        }

        public void Execute()
        {
            using (var streamWriter = new StreamWriter(_outputStream))
            {
                streamWriter.WriteLine();
                

                Dictionary<IPackage, bool> installed = new Dictionary<IPackage, bool>();
                List<IPackage> notInstalled = new List<IPackage>();
                var allPackages = _query.GetLatestVersions(_deployd.PackageSource);
                foreach (var sourcePackage in allPackages)
                {
                    string installPath = _fs.Path.Combine(_deployd.InstallRoot, sourcePackage.Id);
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
                    } else
                    {
                        _logger.DebugFormat("{0} not found", installPath);
                        notInstalled.Add(sourcePackage);
                    }
                }

                streamWriter.WriteLine("Installed packages: (* = updates are available)");
                foreach (var package in installed.Keys)
                {
                    streamWriter.WriteLine("{0}{1}", package.Id, installed[package] ? "*" : "");
                }

                streamWriter.WriteLine();
                streamWriter.WriteLine("Available packages:");
                foreach(var package in notInstalled)
                {
                    streamWriter.WriteLine("{0} {1}", package.Id, package.Version);
                }
            }
        }
    }
}