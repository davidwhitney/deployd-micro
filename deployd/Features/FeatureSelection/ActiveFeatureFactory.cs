using deployd.AppStart;
using deployd.Extensibility.Configuration;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
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
        private readonly ILog _log;

        public ActiveFeatureFactory(IKernel kernel, IInstanceConfiguration instanceConfiguration, ILog log)
        {
            _kernel = kernel;
            _instanceConfiguration = instanceConfiguration;
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
                commandCollection.Add(_kernel.GetService<HelpCommand>());
                return commandCollection;
            }

            if (!_instanceConfiguration.Install)
            {
                commandCollection.Add(_kernel.GetService<HelpCommand>());
                return commandCollection;
                    // TODO: Display info on current version of packages?
            }

            commandCollection.Add(_kernel.GetService<AppLocatingCommand>());
            commandCollection.Add(_kernel.GetService<AppExtractionCommand>());
            commandCollection.Add(_kernel.GetService<AppInstallationCommand>());
            commandCollection.Add(_kernel.GetService<PurgeOldBackupsCommand>());
            return commandCollection;
        }
    }
}
