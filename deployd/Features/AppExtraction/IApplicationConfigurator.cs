namespace deployd.Features.AppExtraction
{
    public interface IApplicationConfigurator
    {
        bool CanConfigure(object packageInfo);
        void Configure(string path, object packageInfo, string forEnvironment);
    }
}