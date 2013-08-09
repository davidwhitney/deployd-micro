using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;
using deployd.Extensibility.Configuration;
using deployd.Features.AppLocating;
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
                streamWriter.WriteLine("Installed packages: (* = updates are available)");

                List<IPackage> installed = new List<IPackage>();
                List<IPackage> notInstalled = new List<IPackage>();
                var allPackages = _query.GetLatestVersions(_deployd.PackageSource);
                foreach (var sourcePackage in allPackages)
                {
                    var appMap = new ApplicationMap(sourcePackage.Id, _fs.Path.Combine(_deployd.InstallRoot, sourcePackage.Id));
                    if (_fs.File.Exists(appMap.VersionFile))
                    {
                        SemanticVersion installedVersion;
                        if (SemanticVersion.TryParse(_fs.File.ReadAllText(appMap.VersionFile), out installedVersion))
                        {
                            streamWriter.Write("{0}", sourcePackage.Id);
                                if (installedVersion < sourcePackage.Version)
                                {
                                    streamWriter.Write("*");
                                }
                            streamWriter.WriteLine();
                            installed.Add(sourcePackage);
                        }
                    } else
                    {
                        notInstalled.Add(sourcePackage);
                    }
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