using System;
using System.IO;
using System.IO.Abstractions;
using deployd.Extensibility.Configuration;
using log4net;

namespace deployd.Features.Environment
{
    public class DefaultEnvironmentApplier : IEnvironmentApplier
    {
        private readonly IFileSystem _fs;
        private readonly ILog _log;
        private readonly TextWriter _output;

        public DefaultEnvironmentApplier(IFileSystem fs, ILog log, TextWriter output)
        {
            _fs = fs;
            _log = log;
            _output = output;
        }

        public bool CanConfigure(object packageInfo, IInstanceConfiguration config)
        {
            return !string.IsNullOrWhiteSpace(config.Environment);
        }

        public void Apply(string path, object packageInfo, string forEnvironment)
        {
            var stagingRoot = _fs.DirectoryInfo.FromDirectoryName(path);
            var configDirectory = _fs.DirectoryInfo.FromDirectoryName(stagingRoot.FullName + "\\config");
            // enumerate all config files which have two part file names i.e: something.config

            var configs = stagingRoot.GetFiles("*.config", SearchOption.TopDirectoryOnly);
            _output.WriteLine("Setting environment to '{0}'", forEnvironment);

            foreach (var config in configs)
            {
                // find other files that could be transforms for this one
                var firstPart = config.Name.Substring(0,config.Name.IndexOf(".config"));
                var transforms = configDirectory.GetFiles(firstPart + ".*.config");
                foreach (var transform in transforms)
                {
                    if (transform.Name.Equals(string.Join(".", firstPart, forEnvironment, "config"), StringComparison.OrdinalIgnoreCase))
                    {
                        _log.DebugFormat("Config file {0} will transform using {1}", config.Name, transform.Name);
                        string tempConfigFilePath = Path.Combine(stagingRoot.FullName, "transformed.config");
                        string args = string.Format("--source={0} --transform={1} --destination={2}",
                                                    config.FullName, transform.FullName,
                                                    tempConfigFilePath);
                        var process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TransformVsConfiguration.exe");
                        process.StartInfo.Arguments = args;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        
                        process.Start();
                        _log.Debug(process.StandardOutput.ReadToEnd());
                        process.WaitForExit();

                        _fs.File.Delete(config.FullName);
                        _fs.File.Move(tempConfigFilePath, config.FullName);
                    }
                }
            }
        }
    }
}