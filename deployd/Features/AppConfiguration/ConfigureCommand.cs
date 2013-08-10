using System;
using System.IO;
using System.Text;
using deployd.Extensibility.Configuration;

namespace deployd.Features.AppConfiguration
{
    public class ConfigureCommand : IFeatureCommand
    {
        private readonly IInstanceConfiguration _config;
        private readonly DeploydConfiguration _deploydConfiguration;
        private readonly DeploydConfigurationManager _configurationManager;
        private readonly Stream _outputStream;

        public ConfigureCommand(IInstanceConfiguration config,
            DeploydConfiguration deploydConfiguration,
            DeploydConfigurationManager configurationManager,
            Stream outputStream)
        {
            _config = config;
            _deploydConfiguration = deploydConfiguration;
            _configurationManager = configurationManager;
            _outputStream = outputStream;
        }

        public void Execute()
        {
            string settingAndValue = _config.SetConfigurationValue;
            string[] split = settingAndValue.Split('=');

            if (split.Length == 1 || split.Length == 2)
            {
                var property = typeof(DeploydConfiguration)
                    .GetProperty(split[0]);

                if (property != null)
                {
                    if (split.Length == 2)
                    {
                        property.SetValue(_deploydConfiguration, split[1], null);
                        _configurationManager.SaveConfig(_deploydConfiguration);
                    }
                    else
                    {
                        using (var writer = new StreamWriter(_outputStream))
                        {
                            writer.WriteLine("{0}={1}",split[0],property.GetValue(_deploydConfiguration, null));
                            writer.Flush();
                        }
                    }
                    return;
                }
                using (var writer = new StreamWriter(_outputStream))
                {
                    writer.WriteLine("No such configuration option '{0}'", split[0]);
                    writer.Flush();
                }
                return;
            }

            throw new ArgumentException("Usage: deployd --set [option]=[value]");

        }
    }
}
