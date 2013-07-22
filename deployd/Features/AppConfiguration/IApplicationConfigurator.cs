using deployd.Extensibility.Configuration;

namespace deployd.Features.AppConfiguration
{
    public interface IApplicationConfigurator
    {
        bool CanConfigure(object packageInfo, IInstanceConfiguration config);
        void Configure(string path, object packageInfo, string forEnvironment);
    }
}