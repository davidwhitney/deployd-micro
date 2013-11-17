using System;
using System.Diagnostics;
using System.IO;
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
        private readonly TextWriter _output;

        public SetEnvironmentCommand(IFileSystem fs, IApplication app, IApplicationMap appMap, IInstanceConfiguration config, ILog log, TextWriter output)
        {
            _fs = fs;
            _app = app;
            _appMap = appMap;
            _config = config;
            _log = log;
            _output = output;
        }

        public void Execute()
        {
            var allConfigPaths = _fs.Directory.GetFiles(_appMap.Staging, "*.config");
            var stagingFolder = _fs.DirectoryInfo.FromDirectoryName(_appMap.Staging);

            foreach (var sourceConfigPath in allConfigPaths)
            {
                string sourceConfigFileName = _fs.Path.GetFileName(sourceConfigPath);

                // ignore app.config file
                if (sourceConfigFileName.Equals("app.config", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                _log.DebugFormat("searching for transforms for {0}", sourceConfigFileName);
                string[] split = sourceConfigFileName.Split('.');
                //if (split.Length != 2)
                //    continue;

                string expectedTransformFilename = string.Format("{0}.{1}{2}", 
                    _fs.Path.GetFileNameWithoutExtension(sourceConfigPath), 
                    _config.Environment,
                    _fs.Path.GetExtension(sourceConfigPath));

                if (sourceConfigFileName.StartsWith(_appMap.AppName, StringComparison.InvariantCultureIgnoreCase))
                {
                    expectedTransformFilename = string.Format("App.{0}.config", _config.Environment);
                }

                string expectedTransformFilePath = _fs.Path.Combine(_appMap.Staging, expectedTransformFilename);

                _log.DebugFormat("looking for {0}", expectedTransformFilename);
                if (_fs.File.Exists(expectedTransformFilePath))
                {
                    // found a transform for this config
                    _output.WriteLine("Activating {0} configuration", _config.Environment);
                    Transform(sourceConfigPath, expectedTransformFilePath);
                }
            }

            // web specific things
            if (_fs.File.Exists(_fs.Path.Combine(_appMap.Staging, _config.Environment+".htaccess"))
                && _fs.File.Exists(_fs.Path.Combine(_appMap.Staging, ".htaccess")))
            {
                _output.WriteLine("Activating {0} .htaccess", _config.Environment);
                using (var file = _fs.File.Open(_fs.Path.Combine(_appMap.Staging, ".htaccess"), FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(_fs.File.ReadAllText(_fs.Path.Combine(_appMap.Staging, _config.Environment + ".htaccess")));
                }
            }

            if (_fs.File.Exists(_fs.Path.Combine(_appMap.Staging, "robots."+_config.Environment + ".txt"))
                && _fs.File.Exists(_fs.Path.Combine(_appMap.Staging, "robots.txt")))
            {
                _output.WriteLine("Activating {0} robots.txt", _config.Environment);
                using (var file = _fs.File.Open(_fs.Path.Combine(_appMap.Staging, "robots.txt"), FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(_fs.File.ReadAllText(_fs.Path.Combine(_appMap.Staging, "robots." + _config.Environment + ".txt")));
                }
            }
        }

        private void Transform(string sourceConfigPath, string transformFilePath, string outputFileName=null)
        {

            var tempFilePath = _fs.Path.Combine(_fs.Path.GetDirectoryName(sourceConfigPath), Guid.NewGuid().ToString());
            var process = new Process();
            process.StartInfo.FileName = "TransformVsConfiguration.exe";
            process.StartInfo.Arguments=string.Format("--source={0} --transform={1} --destination={2}",
                sourceConfigPath, transformFilePath, tempFilePath);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            outputFileName = outputFileName ?? sourceConfigPath;
            if (_fs.File.Exists(outputFileName))
            {
                _fs.File.Delete(outputFileName);
            }
            _fs.File.Move(tempFilePath, outputFileName);
            _log.DebugFormat("Transform process exited with code {0}", process.ExitCode);
            _log.Debug(output);
            _log.InfoFormat("Config transform: {0} -> {1}", _fs.Path.GetFileName(transformFilePath), _fs.Path.GetFileName(outputFileName));
            _output.WriteLine("Config transform: {0} -> {1}", _fs.Path.GetFileName(transformFilePath), _fs.Path.GetFileName(outputFileName));
        }
    }
}