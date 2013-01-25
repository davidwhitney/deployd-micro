using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppInstallation
{
    public class HookFinder
    {
        private readonly IFileSystem _fs;
        private readonly InstanceConfiguration _configuration;

        public HookFinder(IFileSystem fs, InstanceConfiguration configuration)
        {
            _fs = fs;
            _configuration = configuration;
        }

        public Hooks DiscoverHooks()
        {
            var searchPath = _configuration.AppDirectory.Staging;

            var firstInstallHooks = _fs.Directory.GetFiles(searchPath, ".first-install*", SearchOption.AllDirectories);
            var preInstallHooks = _fs.Directory.GetFiles(searchPath, ".pre-install*", SearchOption.AllDirectories);
            var postInstallHooks = _fs.Directory.GetFiles(searchPath, ".post-install*", SearchOption.AllDirectories);
            
            return new Hooks
                {
                    FirstInstall = firstInstallHooks.ToList(),
                    PreInstall = preInstallHooks.ToList(),
                    PostInstall = postInstallHooks.ToList()
                };
        }
    }
}
