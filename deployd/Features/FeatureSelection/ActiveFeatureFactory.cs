using deployd.AppStart;
using deployd.Extensibility.Configuration;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Features.Help;
using deployd.Features.PurgeOldBackups;
using deployd.Features.ShowState;
using log4net;
using Ninject;

namespace deployd.Features.FeatureSelection
{
    public class ActiveFeatureFactory
    {
        private readonly IKernel _kernel;
        private readonly IInstanceConfiguration _instanceConfiguration;
        private readonly ILog _log;
        private readonly ILoggingConfiguration _loggingConfiguration;

        public ActiveFeatureFactory(IKernel kernel, IInstanceConfiguration instanceConfiguration, ILog log, ILoggingConfiguration loggingConfiguration)
        {
            _kernel = kernel;
            _instanceConfiguration = instanceConfiguration;
            _log = log;
            _loggingConfiguration = loggingConfiguration;
        }

        public CommandCollection BuildCommands()
        {
            if (_instanceConfiguration.Verbose)
            {
                _loggingConfiguration.SetLogLevelToDebug();
            }

            var commandCollection = _kernel.GetService<CommandCollection>();

            if (_instanceConfiguration.ShowState)
            {
                commandCollection.Add(_kernel.GetService<ShowStateCommand>());
                return commandCollection;
            }

            if (_instanceConfiguration.Help
                || string.IsNullOrWhiteSpace(_instanceConfiguration.AppName))
            {
                commandCollection.Add(_kernel.GetService<HelpCommand>());
                return commandCollection;
            }

            if (!_instanceConfiguration.Install && !_instanceConfiguration.Prep)
            {
                commandCollection.Add(_kernel.GetService<HelpCommand>());
                return commandCollection;
                    // TODO: Display info on current version of packages?
            }

            if (_instanceConfiguration.Install || _instanceConfiguration.Prep)
            {
                commandCollection.Add(_kernel.GetService<AppLocatingCommand>());
                commandCollection.Add(_kernel.GetService<AppExtractionCommand>());

                if (_instanceConfiguration.Install)
                {
                    commandCollection.Add(_kernel.GetService<AppInstallationCommand>());
                }

                commandCollection.Add(_kernel.GetService<PurgeOldBackupsCommand>());
            }
            return commandCollection;
        }
    }
}
