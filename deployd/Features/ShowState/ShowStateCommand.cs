using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using deployd.Extensibility.Configuration;
using deployd.Features.AppLocating;

namespace deployd.Features.ShowState
{
    public class ShowStateCommand : IFeatureCommand
    {
        private readonly IApplication _app;
        private readonly Stream _outputStream;
        private readonly IEnumerable<IAppInstallationLocator> _finders;
        private readonly IInstanceConfiguration _config;

        public ShowStateCommand(IApplication app,
            Stream outputStream, IEnumerable<IAppInstallationLocator> finders, IInstanceConfiguration config)
        {
            _app = app;
            _outputStream = outputStream;
            _finders = finders;
            _config = config;
        }

        public void Execute()
        {
            using (var streamWriter = new StreamWriter(_outputStream))
            {
                var activeFinders = _finders.Where(x => x.SupportsPathType()).ToList();

                var location =
                    activeFinders.Select(locator => locator.CanFindPackageAsObject(_config.AppName))
                                 .FirstOrDefault(result => result != null);

                if (_app.IsInstalled)
                {
                    var installedVersion = _app.GetInstalledVersion();
                    streamWriter.WriteLine("Installed version: {0}", installedVersion);
                }
                else
                {
                    streamWriter.WriteLine("Package not installed");
                }


                if (location != null)
                {
                    streamWriter.WriteLine("Latest available version: {0}", new Version(location.PackageVersion));
                }
                else
                {
                    streamWriter.WriteLine("Package not found in source");
                }
            }
        }
    }
}