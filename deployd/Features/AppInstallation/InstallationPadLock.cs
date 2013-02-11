using System.IO;
using System.IO.Abstractions;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppInstallation
{
    public class InstallationPadLock
    {
        private readonly InstanceConfiguration _instanceConfiguration;
        private readonly IFileSystem _fs;

        public InstallationPadLock(InstanceConfiguration instanceConfiguration, IFileSystem fs)
        {
            _instanceConfiguration = instanceConfiguration;
            _fs = fs;
        }

        public void LockAppInstallation()
        {
            if (!_fs.File.Exists(_instanceConfiguration.ApplicationMap.Lockfile))
            {
                _fs.File.WriteAllText(_instanceConfiguration.ApplicationMap.Lockfile, string.Empty);
            }
            
            _instanceConfiguration.ApplicationMap.Lock = File.Open(_instanceConfiguration.ApplicationMap.Lockfile,
                                                                   FileMode.Open, FileAccess.Read, FileShare.None);
        }

        public void UnlockAppInstallation()
        {
            if (_instanceConfiguration.ApplicationMap != null
                && _instanceConfiguration.ApplicationMap.Lock != null)
            {
                _instanceConfiguration.ApplicationMap.Lock.Close();
                _instanceConfiguration.ApplicationMap.Lock.Dispose();
                _fs.File.Delete(_instanceConfiguration.ApplicationMap.Lockfile);
            }
        }
    }
}