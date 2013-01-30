using deployd.AppStart;
using deployd.Extensibility.Configuration;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Features.ClientConfiguration;
using deployd.Features.Help;
using deployd.Features.PurgeOldBackups;
using log4net;
using Ninject;

namespace deployd.Features.FeatureSelection
{
    public class ActiveFeatureFactory
    {
        private readonly IKernel _kernel;
        private readonly IInstanceConfiguration _instanceConfiguration;
        private readonly DeploydConfiguration _clientConfig;
        private readonly ILog _log;

        public ActiveFeatureFactory(IKernel kernel, IInstanceConfiguration instanceConfiguration, DeploydConfiguration clientConfig, ILog log)
        {
            _kernel = kernel;
            _instanceConfiguration = instanceConfiguration;
            _clientConfig = clientConfig;
            _log = log;
        }

        public CommandCollection BuildCommands()
        {
            if (_instanceConfiguration.Verbose)
            {
                // yes, for real...
                var heir =
                    (log4net.Repository.Hierarchy.Hierarchy)
                    (((log4net.Repository.Hierarchy.Logger) _log.Logger).Repository);

                ((log4net.Repository.Hierarchy.Logger)_log.Logger).Level = heir.LevelMap["DEBUG"];
            }

            var commandCollection = _kernel.GetService<CommandCollection>();

            if (_instanceConfiguration.Help
                || string.IsNullOrWhiteSpace(_instanceConfiguration.AppName))
            {
                commandCollection.Add(CreateCommand<HelpCommand>());
                return commandCollection;
            }

            if (!_instanceConfiguration.Install)
            {
                commandCollection.Add(CreateCommand<HelpCommand>());
                return commandCollection;
                    // TODO: Display info on current version of packages?
            }

            commandCollection.Add(CreateCommand<AppLocatingCommand>());
            commandCollection.Add(CreateCommand<AppExtractionCommand>());
            commandCollection.Add(CreateCommand<AppInstallationCommand>());
            commandCollection.Add(CreateCommand<PurgeOldBackupsCommand>());
            return commandCollection;
        }

        public TCommand CreateCommand<TCommand>()
            where TCommand : IFeatureCommand
        {
            var command = _kernel.GetService<TCommand>();
            command.DeploydConfiguration = _clientConfig;
            command.Config = _instanceConfiguration;
            return command;
        }
    }
}
