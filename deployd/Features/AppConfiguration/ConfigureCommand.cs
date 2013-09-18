using System;
using System.IO;
using System.Text;
using deployd.Extensibility.Configuration;

namespace deployd.Features.AppConfiguration
{
    public class ConfigureCommand : IFeatureCommand
    {
        private readonly IInstanceConfiguration _config;
        private readonly DeploydConfigurationManager _configurationManager;
        private readonly TextWriter _output;

        public ConfigureCommand(IInstanceConfiguration config,
            DeploydConfigurationManager configurationManager,
            TextWriter output)
        {
            _config = config;
            _configurationManager = configurationManager;
            _output = output;
        }

        public void Execute()
        {
            string settingAndValue = _config.SetConfigurationValue;
            string[] split = settingAndValue.Split('=');

            var deploydConfiguration = _configurationManager.LoadConfig();
            if (split.Length == 1 || split.Length == 2)
            {
                var property = typeof(DeploydConfiguration).GetProperty(split[0]);

                if (property != null)
                {
                    if (split.Length == 2)
                    {
                        if (property.PropertyType.IsEnum)
                        {
                            property.SetValue(deploydConfiguration, Enum.Parse(property.PropertyType, split[1], true), null);
                        }
                        else
                        {
                            property.SetValue(deploydConfiguration, Convert.ChangeType(split[1], property.PropertyType), null);
                        }
                        _configurationManager.SaveConfig(deploydConfiguration);
                    }
                    else
                    {
                        _output.WriteLine("{0}={1}",split[0],property.GetValue(deploydConfiguration, null));
                    }
                    return;
                }
                _output.WriteLine("No such configuration option '{0}'", split[0]);
                return;
            }

            throw new ArgumentException("Usage: deployd --set [option]=[value]");

        }
    }
}
