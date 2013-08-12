using System;
using System.IO;
using System.IO.Abstractions;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using deployd.Features.AppInstallation.Hooks;
using log4net;

namespace deployd.Features.AppInstallation.HookExecution
{
    public class PowershellRunner : IHookRunner
    {
        private readonly IFileSystem _fs;
        private readonly ILog _log;
        private readonly TextWriter _output;

        public PowershellRunner(IFileSystem fs, ILog log, TextWriter output)
        {
            _fs = fs;
            _log = log;
            _output = output;
        }

        public void ExecuteHook(HookTypeRef hookTypeRef, string arguments = null)
        {
            try
            {

                using (var runspace = RunspaceFactory.CreateRunspace())
                {
                    runspace.Open();

                    var runSpaceInvoker = new RunspaceInvoke(runspace);
                    runSpaceInvoker.Invoke("Set-ExecutionPolicy Unrestricted -Scope Process");
                    using (var pipeline = runspace.CreatePipeline())
                    {
                        var command = new Command(hookTypeRef.FileName);
                        pipeline.Commands.Add(command);
                        var results = pipeline.Invoke();
                        foreach (var result in results)
                        {
                            _log.Info(result);
                        }

                        if (pipeline.PipelineStateInfo.State != PipelineState.Completed)
                        {
                            _log.InfoFormat("{0}", pipeline.PipelineStateInfo.Reason);
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
    }
}