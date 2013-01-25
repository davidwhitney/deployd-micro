using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppInstallation
{
    public class AppInstallationCommand : IFeatureCommand
    {
        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }
        public void Execute()
        {
            
        }
    }
}
