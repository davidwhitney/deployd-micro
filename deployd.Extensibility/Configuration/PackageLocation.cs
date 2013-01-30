
namespace deployd.Extensibility.Configuration
{
    public class PackageLocation<TPackageType>
    {
        public TPackageType PackageDetails { get; set; }
        public string PackageVersion { get; set; }
    }
}