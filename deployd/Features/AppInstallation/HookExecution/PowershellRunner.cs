using System;
using System.IO;
using System.IO.Abstractions;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using System.Text;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation.Hooks;
using log4net;

namespace deployd.Features.AppInstallation.HookExecution
{
    public class PowershellRunner : IHookRunner
    {
        private readonly IFileSystem _fs;
        private readonly ILog _log;
        private readonly TextWriter _output;
        private readonly IInstanceConfiguration _instanceConfiguration;
        private readonly IInstallationRoot _installationRoot;

        public PowershellRunner(IFileSystem fs, ILog log, TextWriter output, IInstanceConfiguration instanceConfiguration,
            IInstallationRoot installationRoot)
        {
            _fs = fs;
            _log = log;
            _output = output;
            _instanceConfiguration = instanceConfiguration;
            _installationRoot = installationRoot;
        }

        public void ExecuteHook(HookTypeRef hookTypeRef, string arguments = null)
        {
            _output.WriteLine("Running {0} hook {1}", hookTypeRef.Type, hookTypeRef.FileName);
            string scriptContent = "";
            using (var file = _fs.File.Open(hookTypeRef.FileName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(file))
            {
                scriptContent = reader.ReadToEnd();
                file.Close();
            }

            scriptContent = ReplaceString(scriptContent, "write-host", "write-output", StringComparison.InvariantCultureIgnoreCase);

            try
            {

                using (var runspace = RunspaceFactory.CreateRunspace())
                {
                    runspace.Open();

                    var runSpaceInvoker = new RunspaceInvoke(runspace);
                    runSpaceInvoker.Invoke("Set-ExecutionPolicy Unrestricted -Scope Process");
                    using (var pipeline = runspace.CreatePipeline())
                    {
                        var script = PowerShell.Create();
                        script.Runspace = runspace;
                        script.AddScript(scriptContent);
                        //var command = new Command(hookTypeRef.FileName, true);
                        //pipeline.Commands.Add(command);
                        script.AddParameter("Environment", _instanceConfiguration.Environment);
                        script.AddParameter("InstallationRoot", _installationRoot.Path);

                        var results = script.Invoke();
                        foreach (var result in results)
                        {
                            _output.WriteLine(result);
                        }

                        if (pipeline.PipelineStateInfo.State != PipelineState.Completed)
                        {
                            _output.WriteLine("{0}", pipeline.PipelineStateInfo.Reason);
                        }

                        if (pipeline.Error.Count > 0)
                        {
                            //iterate over Error PipeLine until end
                            while (!pipeline.Error.EndOfPipeline)
                            {
                                //read one PSObject off the pipeline
                                var value = pipeline.Error.Read() as PSObject;
                                if (value != null)
                                {
                                    //get the ErrorRecord
                                    var r = value.BaseObject as ErrorRecord;
                                    if (r != null)
                                    {
                                        _output.WriteLine(r.InvocationInfo.MyCommand.Name + " : " +
                                                         r.Exception.Message);
                                        _output.WriteLine(r.InvocationInfo.PositionMessage);
                                        _output.WriteLine("+ CategoryInfo: {0}", r.CategoryInfo);
                                        _output.WriteLine("+ FullyQualifiedErrorId: {0}", r.FullyQualifiedErrorId);
                                    }
                                }
                            }
                        }
                    }
                    runspace.Close();
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.Message);
                _output.WriteLine(ex.StackTrace);
            }
        }

        public bool SupportsHook(HookTypeRef hookTypeRef)
        {
            return hookTypeRef.Type == HookType.File
                   && _fs.Path.GetExtension(hookTypeRef.FileName)==".ps1";
        }

        public static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }

    
}