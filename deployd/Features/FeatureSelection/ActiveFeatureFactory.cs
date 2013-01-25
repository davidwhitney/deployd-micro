using System.Collections.Generic;
using Ninject;
using deployd.AppStart;
using deployd.Features.ClientConfiguration;
using deployd.Features.Help;

namespace deployd.Features.FeatureSelection
{
    public class ActiveFeatureFactory
    {
        private readonly IKernel _kernel;
        private readonly InstanceConfiguration _instanceConfiguration;
        private readonly Configuration _clientConfig;

        public ActiveFeatureFactory(IKernel kernel, InstanceConfiguration instanceConfiguration, Configuration clientConfig)
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
                return new List<IFeatureCommand> {CreateCommand<HelpCommand>(_clientConfig, _instanceConfiguration)};
            }
            
            return new List<IFeatureCommand>
                {

                };
        }

        public TCommand CreateCommand<TCommand>(Configuration config, InstanceConfiguration instanceConfig)
            where TCommand : IFeatureCommand
        {
            var command = _kernel.GetService<TCommand>();
            command.Configuration = config;
            command.InstanceConfiguration = instanceConfig;
            return command;
        }
    }
}
