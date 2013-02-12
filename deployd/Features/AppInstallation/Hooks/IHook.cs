using deployd.Extensibility.Configuration;

namespace deployd.Features.AppInstallation.Hooks
{
    public interface IHook
    {
        void Execute(IInstanceConfiguration config);
    }
}