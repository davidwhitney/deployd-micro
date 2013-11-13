using System;
using System.IO;
using deployd.AppStart;
using deployd.Extensibility.Configuration;
using deployd.Features.AppConfiguration;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Features.Help;
using deployd.Features.PurgeOldBackups;
using deployd.Features.ShowState;
using deployd.Features.Update;
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
        private readonly TextWriter _output;

        public ActiveFeatureFactory(IKernel kernel, IInstanceConfiguration instanceConfiguration, ILog log, ILoggingConfiguration loggingConfiguration, TextWriter output)
        {
            _kernel = kernel;
            _instanceConfiguration = instanceConfiguration;
            _log = log;
            _loggingConfiguration = loggingConfiguration;
            _output = output;
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

            if (!string.IsNullOrWhiteSpace(_instanceConfiguration.SetConfigurationValue))
            {
                commandCollection.Add(_kernel.GetService<ConfigureCommand>());
                return commandCollection;
            }

            if (_instanceConfiguration.Update)
            {
                if (string.IsNullOrEmpty(_instanceConfiguration.Environment))
                {
                    commandCollection.Add(_kernel.GetService<HelpCommand>());
                }
                else
                {
                    commandCollection.Add(_kernel.GetService<UpdateCommand>());
                }
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
                if (string.IsNullOrEmpty(_instanceConfiguration.Environment))
                {
                    commandCollection.Add(_kernel.GetService<HelpCommand>());
                }
                else
                {
                    commandCollection.Add(_kernel.GetService<HelpCommand>());
                }
                return commandCollection;
            }

            if (_instanceConfiguration.Install && _instanceConfiguration.Prep)
            {
                throw new ArgumentException("You can specify the install (-i) or prepare (-p) arguments, but not both");
            }

            if (_instanceConfiguration.Install || _instanceConfiguration.Prep)
            {
                if (string.IsNullOrEmpty(_instanceConfiguration.Environment))
                {
                    _output.WriteLine("Specify an environment");
                    commandCollection.Clear();
                    return commandCollection;
                }
                
                commandCollection.Add(_kernel.GetService<AppLocatingCommand>());
                commandCollection.Add(_kernel.GetService<AppExtractionCommand>());

                if (_instanceConfiguration.Install)
                {
                    _output.WriteLine("Application will be installed");
                    commandCollection.Add(_kernel.GetService<SetEnvironmentCommand>());
                    commandCollection.Add(_kernel.GetService<AppInstallationCommand>());
                }
                else
                {
                    _output.WriteLine("Application will be prepared for installation only");
                }

                commandCollection.Add(_kernel.GetService<PurgeOldBackupsCommand>());
            }
            return commandCollection;
        }
    }
}
