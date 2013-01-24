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
        private readonly ArgumentParser _argumentParser;
        private readonly ClientConfigurationManager _clientConfigurationManager;

        public ActiveFeatureFactory(IKernel kernel, ArgumentParser argumentParser, ClientConfigurationManager clientConfigurationManager)
        {
            _kernel = kernel;
            _argumentParser = argumentParser;
            _clientConfigurationManager = clientConfigurationManager;
        }

        public IEnumerable<IFeatureCommand> BuildCommandsFor(string[] commandLineArguments)
        {
            var configuration = _clientConfigurationManager.LoadConfig();
            var instanceConfiguration = _argumentParser.Parse(commandLineArguments);

            if (instanceConfiguration.Help)
            {
                return new List<IFeatureCommand> {CreateCommand<HelpCommand>(configuration, instanceConfiguration)};
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
