using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.PurgeOldBackups
{
    public class PurgeOldBackupsCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        private readonly InstanceConfiguration _config;

        public DeploydConfiguration DeploydConfiguration { get; set; }
        public InstanceConfiguration Config { get; set; }

        public PurgeOldBackupsCommand(IFileSystem fs, InstanceConfiguration config)
        {
            _fs = fs;
            _config = config;
        }

        public void Execute()
        {
            var backups = _fs.Directory.GetDirectories(_config.DirectoryMaps.FullPath);

            var totalBackupsToKeep = 10;

            if (backups.Length <= 10)
            {
                return;
            }

            var oldestFirst = backups.Reverse().ToArray();
            var itemsToRemove = oldestFirst.Skip(totalBackupsToKeep + 1).ToList();

            foreach (var item in itemsToRemove)
            {
                _fs.Directory.Delete(item, true);
            }
        }
    }
}
