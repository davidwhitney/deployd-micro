using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }
        public void Execute()
        {
            
        }
    }
}
