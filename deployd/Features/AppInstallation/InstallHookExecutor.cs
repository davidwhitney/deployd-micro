using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using deployd.Features.AppInstallation.Hooks;
using deployd.Features.FeatureSelection;
using log4net;

namespace deployd.Features.AppInstallation
{
    public class InstallHookExecutor
    {
        private readonly ILog _log;
        private readonly InstanceConfiguration _config;
        private readonly HookFinder _finder;
        private readonly Lazy<Hooks.Hooks> _hooks;

        public InstallHookExecutor(HookFinder finder, ILog log, InstanceConfiguration config)
        {
            _log = log;
            _config = config;
            _finder = finder;
            _hooks = new Lazy<Hooks.Hooks>(() => _finder.DiscoverHooks());
        }

        public void ExecuteFirstInstall()
        {
            RunHooks(_hooks.Value.FirstInstall);
        }

        public void ExecutePreInstall()
        {
            RunHooks(_hooks.Value.PreInstall);
        }

        public void ExecutePostInstall()
        {
            RunHooks(_hooks.Value.PostInstall);
        }

        private void RunHooks(IEnumerable<Hook> hookFiles)
        {
            foreach (var hook in hookFiles.Where(x=>x.Type == HookType.File))
            {
                _log.Info("Executing package hook: " + hook);
                ExecuteHook(hook.FileName);
            }

            foreach (var hook in hookFiles.Where(x=>x.Type == HookType.Class))
            {
                _log.Info("Executing plugin hook: " + hook);
                var classs = (IHook)Activator.CreateInstance(hook.GetType());
                classs.Execute(_config);
            }
        }

        private void ExecuteHook(string hook)
        {
            var startInfo = new ProcessStartInfo
                {
                    FileName = hook,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

            var envrs = _config.ApplicationMap.GetType().GetProperties()
                               .Select(fi => new {Field = fi.Name, Value = fi.GetValue(_config.ApplicationMap)})
                               .ToList();

            foreach (var variable in envrs)
            {
                startInfo.EnvironmentVariables.Add("Deployd." + variable.Field, variable.Value.ToString());
            }

            BuildInterpreterPrefixes(hook, startInfo);

            var process = Process.Start(startInfo);

            using (var outputStream = process.StandardOutput)
            {
                process.WaitForExit();
                _log.Info(outputStream.ReadToEnd());
            }

            if (process.ExitCode != 0)
            {
                throw new Exception("Execution of hook failed. Exit code " + process.ExitCode);
            }
        }

        private static void BuildInterpreterPrefixes(string hook, ProcessStartInfo startInfo)
        {
            if (hook.EndsWith(".ps1"))
            {
                startInfo.FileName = "powershell " + startInfo.FileName;
            }
            else if (hook.EndsWith(".rb"))
            {
                startInfo.FileName = "ruby " + startInfo.FileName;
            }
            else if (hook.EndsWith(".py"))
            {
                startInfo.FileName = "python " + startInfo.FileName;
            }
            else if (hook.EndsWith(".cgi"))
            {
                startInfo.FileName = "perl " + startInfo.FileName;
            }
            else if (hook.EndsWith(".php"))
            {
                startInfo.FileName = "php " + startInfo.FileName;
            }
            else if (hook.EndsWith(".js"))
            {
                startInfo.FileName = "node " + startInfo.FileName;
            }
        }
    }
}