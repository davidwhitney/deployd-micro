namespace deployd.Features.ClientConfiguration
{
    public class Configuration
    {
        public PackageSourceType SourceType { get; set; }
        public string PackageSource { get; set; }
    }

    public enum PackageSourceType
    {
        FileSystem
    }
}
