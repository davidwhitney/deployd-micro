namespace deployd.Features.ClientConfiguration
{
    public class Configuration
    {
        public PackageType PackageType { get; set; }
        public PackageSourceType SourceType { get; set; }
        public string PackageSource { get; set; }
        public bool DefaultConfiguration { get; set; }
    }
}
