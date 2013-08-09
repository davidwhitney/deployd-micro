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

namespace deployd.Features.Update
{
    public class UpdateCommand : IFeatureCommand
    {
        private readonly IEnumerable<IAppInstallationLocator> _finders;
        private readonly IInstanceConfiguration _instanceConfiguration;
        private readonly Stream _outputStream;
        private readonly IListLatestVersionsOfPackagesQuery _query;
        private readonly DeploydConfiguration _deployd;
        private readonly IPackageCache _packageCache;
        private readonly ILog _log = LogManager.GetLogger(typeof(UpdateCommand));

        public UpdateCommand(IEnumerable<IAppInstallationLocator> finders, IInstanceConfiguration instanceConfiguration,
             Stream outputStream, IListLatestVersionsOfPackagesQuery query, DeploydConfiguration deployd,
            IPackageCache packageCache)
        {
            _finders = finders;
            _instanceConfiguration = instanceConfiguration;
            _outputStream = outputStream;
            _query = query;
            _deployd = deployd;
            _packageCache = packageCache;
        }

        public void Execute()
        {
            using (var streamWriter = new StreamWriter(_outputStream))
            {
                var activeFinders = _finders.Where(x => x.SupportsPathType()).ToList();

                var location =
                    activeFinders.Select(locator => locator.CanFindPackageAsObject(_instanceConfiguration.AppName))
                                 .FirstOrDefault(result => result != null);

                streamWriter.WriteLine("All packages in {0}:", _deployd.PackageSource);
                var allPackages = _query.GetLatestVersions(_deployd.PackageSource);
                foreach (var package in allPackages)
                {
                    var cachedPackage = _packageCache.GetPackage(package.Id);
                    if (cachedPackage != null)
                    {
                        streamWriter.WriteLine("{0}: {1}", cachedPackage.Id, cachedPackage.Version);
                        streamWriter.WriteLine("Local version: {0}", cachedPackage.Version);
                        streamWriter.WriteLine("Remote version: {0}", package.Version);

                        if (package.Version > cachedPackage.Version)
                        {
                            PrepareInstall(package, _instanceConfiguration.Prep);
                        }
                    }
                }

            }
        }

        private void PrepareInstall(IPackage package, bool prepareOnly)
        {
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
            process.OutputDataReceived += (sender, args) => _log.Info(args.Data);
            process.ErrorDataReceived += (sender, args) => _log.Warn(args.Data);
            process.Exited += (sender, args) => _log.DebugFormat("Process exited with code {0}", process.ExitCode);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
    }
}
