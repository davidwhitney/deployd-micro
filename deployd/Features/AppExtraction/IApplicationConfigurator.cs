using deployd.Extensibility.Configuration;

namespace deployd.Features.AppExtraction
{
    public interface IApplicationConfigurator
    {
        bool CanConfigure(object packageInfo, IInstanceConfiguration config);
        void Configure(string path, object packageInfo, string forEnvironment);
    }
}