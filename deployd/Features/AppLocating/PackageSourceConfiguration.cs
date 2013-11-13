using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;

namespace deployd.Features.AppLocating
{
    public interface IPackageSourceConfiguration
    {
        string PackageSource { get; }
    }

    public class PackageSourceConfiguration : IPackageSourceConfiguration
    {
        private readonly DeploydConfiguration _configuration;
        private readonly IInstanceConfiguration _instanceConfiguration;

        public PackageSourceConfiguration(DeploydConfiguration configuration, IInstanceConfiguration instanceConfiguration)
        {
            _configuration = configuration;
            _instanceConfiguration = instanceConfiguration;
        }

        private string _source = null;
        public string PackageSource
        {
            get
            {
                if (_source == null)
                {
                    _source = _instanceConfiguration.PackageSource ?? _configuration.PackageSource;
                    if (!_source.StartsWith("http"))
                    {
                        _source = _source.ToAbsolutePath();
                    }
                }
                return _source;
            }
        }
    }
}
