using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features
{
    public interface IFeatureCommand
    {
        DeploydConfiguration DeploydConfiguration { get; set; }
        InstanceConfiguration Config { get; set; }
        void Execute();
    }
}
