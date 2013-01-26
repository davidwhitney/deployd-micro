using deployd.AppStart;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Features.ClientConfiguration;
using deployd.Features.Help;
using log4net;
using Ninject;

namespace deployd.Features.FeatureSelection
{
    public class ActiveFeatureFactory
    {
        private readonly IKernel _kernel;
        private readonly InstanceConfiguration _instanceConfiguration;
        private readonly DeploydConfiguration _clientConfig;
        private readonly ILog _log;

        public ActiveFeatureFactory(IKernel kernel, InstanceConfiguration instanceConfiguration, DeploydConfiguration clientConfig, ILog log)
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

            if (_instanceConfiguration.Help
                || string.IsNullOrWhiteSpace(_instanceConfiguration.AppName))
            {
                return new CommandCollection(_log) { CreateCommand<HelpCommand>() };
            }

            if (!_instanceConfiguration.Install)
            {
                return new CommandCollection(_log) { CreateCommand<HelpCommand>() };
                    // TODO: Display info on current version of packages?
            }

            return new CommandCollection(_log)
                {
                    CreateCommand<AppLocatingCommand>(),
                    CreateCommand<AppExtractionCommand>(),
                    CreateCommand<AppInstallationCommand>(),
                };
        }

        public TCommand CreateCommand<TCommand>()
            where TCommand : IFeatureCommand
        {
            var command = _kernel.GetService<TCommand>();
            command.DeploydConfiguration = _clientConfig;
            command.InstanceConfiguration = _instanceConfiguration;
            return command;
        }
    }
}
