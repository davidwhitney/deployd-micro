using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using deployd.Extensibility.Hooks;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppInstallation.Hooks
{
    public class HookFinder : IHookFinder
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
            var searchPath = _configuration.ApplicationMap.Staging;

            var firstInstallHooks = _fs.Directory.GetFiles(searchPath, "hook-first-install*", SearchOption.AllDirectories);
            var preInstallHooks = _fs.Directory.GetFiles(searchPath, "hook-pre-install*", SearchOption.AllDirectories);
            var postInstallHooks = _fs.Directory.GetFiles(searchPath, "hook-post-install*", SearchOption.AllDirectories);
            var postFirstInstallHooks = _fs.Directory.GetFiles(searchPath, "hook-post-first-install*", SearchOption.AllDirectories);
            
            var hooks = new Hooks
                {
                    FirstInstall = firstInstallHooks.Select(x=> new HookTypeRef(x, HookType.File)).ToList(),
                    PreInstall = preInstallHooks.Select(x => new HookTypeRef(x, HookType.File)).ToList(),
                    PostInstall = postInstallHooks.Select(x => new HookTypeRef(x, HookType.File)).ToList(),
                    PostFirstInstall = postFirstInstallHooks.Select(x=>new HookTypeRef(x, HookType.File)).ToList()
                };

            foreach (var hook in hooks.PostInstall.Where(x=>x.Type == HookType.File))
            {
                hook.FileName = hook.FileName.Replace(_configuration.ApplicationMap.Staging,
                                                      _configuration.ApplicationMap.Active);
            }
            foreach (var hook in hooks.PostFirstInstall.Where(x => x.Type == HookType.File))
            {
                hook.FileName = hook.FileName.Replace(_configuration.ApplicationMap.Staging,
                                                      _configuration.ApplicationMap.Active);
            }

            AddAssemblyPlugins(hooks);

            return hooks;
        }

        private void AddAssemblyPlugins(Hooks hooks)
        {
            var deploydPath = Path.GetDirectoryName(GetType().Assembly.Location);
            foreach (var file in _fs.Directory.GetFiles(deploydPath, "*Deployd.Plugins*.dll"))
            {
                Assembly.LoadFile(file);
            }

            var pluginAssemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Deployd.Plugins")).ToList();

            foreach (var allTypes in pluginAssemblies.Select(assembly => assembly.GetTypes()))
            {
                hooks.FirstInstall.AddRange(
                    allTypes.Where(x => x.GetInterfaces().Any(y => y.Name == typeof (IFirstInstallHook).Name))
                            .Select(x => new HookTypeRef(x)));

                hooks.PreInstall.AddRange(
                    allTypes.Where(x => x.GetInterfaces().Any(y => y.Name == typeof (IPreInstallHook).Name))
                            .Select(x => new HookTypeRef(x)));

                hooks.PostInstall.AddRange(
                    allTypes.Where(x => x.GetInterfaces().Any(y => y.Name == typeof (IPostInstallHook).Name))
                            .Select(x => new HookTypeRef(x)));

                hooks.PostFirstInstall.AddRange(
                    allTypes.Where(x => x.GetInterfaces().Any(y => y.Name == typeof (IPostFirstInstallHook).Name))
                            .Select(x => new HookTypeRef(x)));
            }
        }
    }
}
