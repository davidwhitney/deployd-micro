using System;
using System.Diagnostics;
using System.IO.Abstractions;
using deployd.Extensibility.Configuration;
using log4net;

namespace deployd.Features.FeatureSelection
{
    public class SetEnvironmentCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        private readonly IApplication _app;
        private readonly IApplicationMap _appMap;
        private readonly IInstanceConfiguration _config;
        private readonly ILog _log;

        public SetEnvironmentCommand(IFileSystem fs, IApplication app, IApplicationMap appMap, IInstanceConfiguration config, ILog log)
        {
            _fs = fs;
            _app = app;
            _appMap = appMap;
            _config = config;
            _log = log;
        }

        public void Execute()
        {
            var allConfigPaths = _fs.Directory.GetFiles(_appMap.Staging, "*.config");
            var stagingFolder = _fs.DirectoryInfo.FromDirectoryName(_appMap.Staging);

            foreach (var sourceConfigPath in allConfigPaths)
            {
                string sourceConfigFileName = _fs.Path.GetFileName(sourceConfigPath);
                _log.DebugFormat("searching for transforms for {0}", sourceConfigFileName);
                string[] split = sourceConfigFileName.Split('.');
                if (split.Length != 2)
                    continue;

                string expectedTransformFilename = string.Join(".", split[0], _config.Environment, split[1]);
                string expectedTransformFilePath = _fs.Path.Combine(_appMap.Staging, expectedTransformFilename);
                _log.DebugFormat("looking for {0}", expectedTransformFilename);
                if (_fs.File.Exists(expectedTransformFilePath))
                {
                    // found a transform for this config
                    _log.InfoFormat("Found a config transform: {0} and {1}", sourceConfigFileName, expectedTransformFilename);
                    Transform(sourceConfigPath, expectedTransformFilePath);
                }
            }

        }

        private void Transform(string sourceConfigPath, string transformFilePath)
        {
            var tempFilePath = _fs.Path.Combine(_fs.Path.GetDirectoryName(sourceConfigPath), Guid.NewGuid().ToString());
            _fs.File.Move(sourceConfigPath,tempFilePath);
            var process = new Process();
            process.StartInfo.FileName = "TransformVsConfiguration.exe";
            process.StartInfo.Arguments=string.Format("--source={0} --transform={1} --destination={2}",
                tempFilePath, transformFilePath, sourceConfigPath);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            _log.InfoFormat("Transform process exited with code {0}", process.ExitCode);
            _log.Info(output);

        }
    }
}