using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation;
using deployd.Infrastructure;
using log4net;

namespace deployd.Features
{
    public class Application : IApplication
    {
        private readonly IFileSystem _fs;
        private readonly IApplicationMap _appMap;
        private readonly ILog _log;
        private readonly IInstanceConfiguration _config;
        private readonly IInstallationPadLock _installationLock;
        private readonly TextWriter _output;

        public bool IsInstalled { get { return _fs.File.Exists(_appMap.VersionFile); } }
        public bool IsStaged { get { return _fs.Directory.Exists(_appMap.Staging); } }
        
        private const int TotalBackupsToKeep = 10;

        public Application(IApplicationMap appMap, IFileSystem fs, ILog log, IInstanceConfiguration config, IInstallationPadLock installationLock, TextWriter output)
        {
            _fs = fs;
            _appMap = appMap;
            _log = log;
            _config = config;
            _installationLock = installationLock;
            _output = output;

            _output.WriteLine("App directory: " + _appMap.InstallPath);
        }

        public void EnsureDataDirectoriesExist()
        {
            _fs.EnsureDirectoryExists(_appMap.InstallPath);
            _fs.EnsureDirectoryExists(_appMap.Staging);
            _fs.EnsureDirectoryExists(_appMap.CachePath);
            var cacheFolder = _fs.DirectoryInfo.FromDirectoryName(_appMap.CachePath);
            cacheFolder.Attributes = FileAttributes.Hidden;
        }

        public void LockForInstall()
        {
            _installationLock.LockAppInstallation();            
        }

        public void BackupAndInstall()
        {
            BackupCurrentVersion();
            ActivateStaging();
            WriteUpdatedManifest(_config.PackageLocation.PackageVersion);
        }

        public void ActivateStaging()
        {
             _output.WriteLine("Activating staged install at {0}...", _appMap.InstallPath);

            RecursiveDelete(_appMap.InstallPath);
            RecursiveCopy(_appMap.Staging, _appMap.InstallPath);
            RecursiveDelete(_appMap.Staging);
        }

        private void RecursiveDelete(string path)
        {
            var sourceFolder = _fs.DirectoryInfo.FromDirectoryName(path);
            var folders = sourceFolder.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders)
            {
                if (folder.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;

                RecursiveDelete(folder.FullName);
            }
            var files = sourceFolder.GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }
        }

        private void RecursiveCopy(string source, string destination)
        {
            var sourceFolder = _fs.DirectoryInfo.FromDirectoryName(source);
            var folders = sourceFolder.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders)
            {
                if (folder.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;

                RecursiveCopy(folder.FullName, _fs.Path.Combine(destination, folder.Name));

            }

            if (!_fs.Directory.Exists(destination))
                _fs.Directory.CreateDirectory(destination);
            
            var files = sourceFolder.GetFiles();
            foreach (var file in files)
            {
                _fs.File.Copy(file.FullName, _fs.Path.Combine(destination, file.Name));
            }
        }

        public void BackupCurrentVersion()
        {
            if (!IsInstalled)
            {
                // No version file? No previous install!
                return;
            }

            var currentInstalledVersion = GetInstalledVersion();
            var backupPath = Path.Combine(_appMap.CachePath, currentInstalledVersion.ToString());

            if (_fs.Directory.Exists(backupPath))
            {
                var newPath = backupPath + "-duplicate-" + Guid.NewGuid().ToString();
                //_fs.Directory.CreateDirectory(newPath);
                //RecursiveCopy(backupPath, newPath);
                _fs.Directory.Move(backupPath, newPath);
            }

            if (_fs.Directory.Exists(_appMap.InstallPath))
            {
                _output.WriteLine("Backing up current installation...");
                RecursiveCopy(_appMap.InstallPath, backupPath);
            }
        }

        public void WriteUpdatedManifest(string newVersion)
        {
            _fs.File.WriteAllText(_appMap.VersionFile, newVersion);
        }

        public void PruneBackups()
        {
            var backups = _fs.Directory.GetDirectories(_appMap.CachePath);
            if (backups.Length <= 10)
            {
                return;
            }

            var oldestFirst = backups.Reverse().ToArray();
            var itemsToRemove = oldestFirst.Skip(TotalBackupsToKeep).ToList();

            foreach (var item in itemsToRemove)
            {
                _fs.Directory.Delete(item, true);
            }
        }

        public Version GetInstalledVersion()
        {
            return new Version(_fs.File.ReadAllText(_appMap.VersionFile));
        }

        public Version GetLatestAvailableVersion()
        {
            throw new NotImplementedException();
        }
    }
}