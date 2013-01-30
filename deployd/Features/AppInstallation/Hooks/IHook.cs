using deployd.Features.FeatureSelection;

namespace deployd.Features.AppInstallation.Hooks
{
    public interface IHook
    {
        void Execute(InstanceConfiguration config);
    }
}