using System;
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

        public PowershellRunner(IFileSystem fs, ILog log)
        {
            _fs = fs;
            _log = log;
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
                        Console.WriteLine("Credentials for script execution are required");
                        Console.Write("Username: ");
                        var username = Console.ReadLine();
                        Console.Write("Password: ");
                        var password = Console.ReadLine();

                        var command = new Command(hookTypeRef.FileName);
                        command.Parameters.Add("username",username);
                        command.Parameters.Add("password",password);
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
                                        //build whatever kind of message your want
                                        _log.Info(r.InvocationInfo.MyCommand.Name + " : " + r.Exception.Message);
                                        _log.Info(r.InvocationInfo.PositionMessage);
                                        _log.Info(string.Format("+ CategoryInfo: {0}", r.CategoryInfo));
                                        _log.Info(string.Format("+ FullyQualifiedErrorId: {0}", r.FullyQualifiedErrorId));
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
                _log.Error("error", ex);
            }
        }

        public bool SupportsHook(HookTypeRef hookTypeRef)
        {
            return hookTypeRef.Type == HookType.File
                   && _fs.Path.GetExtension(hookTypeRef.FileName)==".ps1";
        }
    }
}