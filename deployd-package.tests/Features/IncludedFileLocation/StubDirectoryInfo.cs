using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;

namespace deployd_package.tests.Features.IncludedFileLocation
{
    public class StubDirectoryInfo : DirectoryInfoBase
    {
        private readonly Dictionary<string, string[]> _filesOnDisk;

        public StubDirectoryInfo(Dictionary<string, string[]> filesOnDisk)
        {
            _filesOnDisk = filesOnDisk;
        }

        public override FileInfoBase[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            var fi = new List<FileInfoBase>();
            foreach (var dir in _filesOnDisk.Values)
            {
                foreach (var file in dir)
                {
                    fi.Add(new FiBaseStub(file));
                }

            }

            return fi.ToArray();
        }

        #region NotImplemented
        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        public override FileAttributes Attributes { get; set; }
        public override DateTime CreationTime { get; set; }
        public override DateTime CreationTimeUtc { get; set; }

        public override bool Exists
        {
            get { throw new NotImplementedException(); }
        }

        public override string Extension
        {
            get { throw new NotImplementedException(); }
        }

        public override string FullName
        {
            get { throw new NotImplementedException(); }
        }

        public override DateTime LastAccessTime { get; set; }
        public override DateTime LastAccessTimeUtc { get; set; }
        public override DateTime LastWriteTime { get; set; }
        public override DateTime LastWriteTimeUtc { get; set; }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override void Create()
        {
            throw new NotImplementedException();
        }

        public override void Create(DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase CreateSubdirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase CreateSubdirectory(string path, DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(bool recursive)
        {
            throw new NotImplementedException();
        }

        public override DirectorySecurity GetAccessControl()
        {
            throw new NotImplementedException();
        }

        public override DirectorySecurity GetAccessControl(AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase[] GetDirectories()
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase[] GetDirectories(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override FileInfoBase[] GetFiles()
        {
            throw new NotImplementedException();
        }

        public override FileInfoBase[] GetFiles(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override FileSystemInfoBase[] GetFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public override FileSystemInfoBase[] GetFileSystemInfos(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override void MoveTo(string destDirName)
        {
            throw new NotImplementedException();
        }

        public override void SetAccessControl(DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase Parent
        {
            get { throw new NotImplementedException(); }
        }

        public override DirectoryInfoBase Root
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
    }
}