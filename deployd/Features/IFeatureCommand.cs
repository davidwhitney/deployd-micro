using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features
{
    public interface IFeatureCommand
    {
        Configuration Configuration { get; set; }
        InstanceConfiguration InstanceConfiguration { get; set; }
        void Execute();
    }
}
