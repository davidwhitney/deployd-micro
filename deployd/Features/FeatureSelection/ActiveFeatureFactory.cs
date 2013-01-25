using deployd.AppStart;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Features.ClientConfiguration;
using deployd.Features.Help;
using Ninject;
using System.Collections.Generic;

namespace deployd.Features.FeatureSelection
{
    public class ActiveFeatureFactory
    {
        private readonly IKernel _kernel;
        private readonly InstanceConfiguration _instanceConfiguration;
        private readonly DeploydConfiguration _clientConfig;

        public ActiveFeatureFactory(IKernel kernel, InstanceConfiguration instanceConfiguration, DeploydConfiguration clientConfig)
        {
            _kernel = kernel;
            _instanceConfiguration = instanceConfiguration;
            _clientConfig = clientConfig;
        }

        public IEnumerable<IFeatureCommand> BuildCommands()
        {
            if (_instanceConfiguration.Help
                || string.IsNullOrWhiteSpace(_instanceConfiguration.AppName))
            {
                return new List<IFeatureCommand> {CreateCommand<HelpCommand>()};
            }

            if (!_instanceConfiguration.Install)
            {
                return new List<IFeatureCommand> {CreateCommand<HelpCommand>()};
                    // TODO: Display info on current version of packages?
            }

            return new List<IFeatureCommand>
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
