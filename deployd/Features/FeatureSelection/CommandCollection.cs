using System;
using System.Collections.Generic;
using System.IO;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using log4net;

namespace deployd.Features.FeatureSelection
{
    public class CommandCollection : List<IFeatureCommand>
    {
        private readonly ILog _log;
        private readonly IInstallationPadLock _padlock;
        private readonly TextWriter _output;

        public CommandCollection(ILog log, IInstallationPadLock padlock, TextWriter output)
        {
            _log = log;
            _padlock = padlock;
            _output = output;
        }

        public int RunAll()
        {
            try
            {
                foreach (var command in this)
                {
                    command.Execute();
                }
            }
            catch (NoPackageFoundException ex)
            {
                _output.WriteLine(ex.Message);
                return -2;
            }
            catch (HookFailureException hookFailure)
            {
                _output.WriteLine(hookFailure.ToString());
                return hookFailure.ExitCode;
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.ToString());
                return -1;
            }
            finally
            {
                _padlock.UnlockAppInstallation();
            }

            return 0;
        }

    }
}