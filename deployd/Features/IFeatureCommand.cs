using deployd.Extensibility.Configuration;
using deployd.Features.ClientConfiguration;

namespace deployd.Features
{
    public interface IFeatureCommand
    {
        DeploydConfiguration DeploydConfiguration { get; set; }
        IInstanceConfiguration Config { get; set; }
        void Execute();
    }
}
