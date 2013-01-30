using deployd.Extensibility.Configuration;

namespace deployd.Extensibility.Hooks
{
    public interface IHook
    {
        void Execute(IInstanceConfiguration config);
    }
}