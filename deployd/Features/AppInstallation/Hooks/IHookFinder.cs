namespace deployd.Features.AppInstallation.Hooks
{
    public interface IHookFinder
    {
        Hooks DiscoverHooks();
    }
}