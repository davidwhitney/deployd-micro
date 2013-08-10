using deployd.Extensibility.Configuration;

namespace deployd.Features.Environment
{
    public interface IEnvironmentApplier
    {
        bool CanConfigure(object packageInfo, IInstanceConfiguration config);
        void Apply(string path, object packageInfo, string forEnvironment);
    }
}