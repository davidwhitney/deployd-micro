using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppLocating
{
    public class AppLocatingCommand : IFeatureCommand
    {
        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }
        public void Execute()
        {
            
        }
    }
}
