using System;
using System.Collections.Generic;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using log4net;

namespace deployd.Features.FeatureSelection
{
    public class CommandCollection : List<IFeatureCommand>
    {
        private readonly ILog _log;
        private readonly IInstallationPadLock _padlock;

        public CommandCollection(ILog log, IInstallationPadLock padlock)
        {
            _log = log;
            _padlock = padlock;
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
                _log.Info(ex.Message);
                return -2;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
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