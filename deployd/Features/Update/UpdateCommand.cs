using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NuGet;
using deployd.Extensibility.Configuration;
using deployd.Features.AppExtraction;
using deployd.Features.AppLocating;
using log4net;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.Features.Update
{
    public class UpdateCommand : IFeatureCommand
    {
        private readonly IEnumerable<IAppInstallationLocator> _finders;
        private readonly IInstanceConfiguration _instanceConfiguration;
        private readonly TextWriter _output;
        private readonly IListLatestVersionsOfPackagesQuery _query;
        private readonly DeploydConfiguration _deployd;
        private readonly IInstallationRoot _installationRoot;
        private readonly IFileSystem _fs;
        private readonly ILog _log = LogManager.GetLogger(typeof(UpdateCommand));

        public UpdateCommand(IEnumerable<IAppInstallationLocator> finders, IInstanceConfiguration instanceConfiguration,
             TextWriter output, IListLatestVersionsOfPackagesQuery query, DeploydConfiguration deployd,
            IInstallationRoot installationRoot, System.IO.Abstractions.IFileSystem fs)
        {
            _finders = finders;
            _instanceConfiguration = instanceConfiguration;
            _output = output;
            _query = query;
            _deployd = deployd;
            _installationRoot = installationRoot;
            _fs = fs;
        }

        public void Execute()
        {
            List<IPackage> packagesToUpdate = new List<IPackage>();
                var activeFinders = _finders.Where(x => x.SupportsPathType()).ToList();

                var location =
                    activeFinders.Select(locator => locator.CanFindPackageAsObject(_instanceConfiguration.AppName))
                        .FirstOrDefault(result => result != null);

                var allPackages = _query.GetLatestVersions(_deployd.PackageSource);
                foreach (var package in allPackages)
                {
                    var appmap = new ApplicationMap(package.Id, _fs.Path.Combine(_installationRoot.Path, package.Id));
                    if (!File.Exists(appmap.VersionFile))
                    {
                        _log.DebugFormat("{0} is not installed", package.Id);
                        continue;
                    }

                    SemanticVersion installedVersion;
                    if (SemanticVersion.TryParse(File.ReadAllText(appmap.VersionFile), out installedVersion))
                    {
                        if (package.Version > installedVersion)
                        {
                            _output.WriteLine(package.Id + " is out of date");
                            packagesToUpdate.Add(package);
                        }
                    }
                }

                if (packagesToUpdate.Count == 0)
                {
                    _output.WriteLine("Everything up to date");
                    return;
                }

                foreach (var package in packagesToUpdate)
                {
                    PrepareInstall(package, _instanceConfiguration.Prep);
                }
        }

        private void PrepareInstall(IPackage package, bool prepareOnly)
        {
            _output.WriteLine("Updating {0}...", package.Id);
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = "deployd.exe";
            process.StartInfo.Arguments = string.Format("-{2} -app=\"{0}\" -e=\"{1}\" {3}",
                package.Id,
                _instanceConfiguration.Environment,
                prepareOnly ? "p" : "i",
                _instanceConfiguration.ForceDownload ? "-f" : "");
            _log.DebugFormat("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
            process.OutputDataReceived += (sender, args) => _output.WriteLine(args.Data);
            process.ErrorDataReceived += (sender, args) => _output.WriteLine(args.Data);
            process.Exited += (sender, args) => _log.DebugFormat("Process exited with code {0}", process.ExitCode);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}
