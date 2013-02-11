using System.IO;
using System.IO.Abstractions;
using deployd.Extensibility.Configuration;

namespace deployd.Features.AppInstallation
{
    public class InstallationPadLock : IInstallationPadLock
    {
        private readonly IApplicationMap _appMap;
        private readonly IFileSystem _fs;

        public InstallationPadLock(IApplicationMap appMap, IFileSystem fs)
        {
            _appMap = appMap;
            _fs = fs;
        }

        public void LockAppInstallation()
        {
            if (!_fs.File.Exists(_appMap.Lockfile))
            {
                _fs.File.WriteAllText(_appMap.Lockfile, string.Empty);
            }

            _appMap.Lock = File.Open(_appMap.Lockfile, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        public void UnlockAppInstallation()
        {
            if (_appMap == null || _appMap.Lock == null)
            {
                return;
            }

            _appMap.Lock.Close();
            _appMap.Lock.Dispose();
            _fs.File.Delete(_appMap.Lockfile);
        }
    }
}