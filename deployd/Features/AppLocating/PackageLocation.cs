
using System;

namespace deployd.Features.AppLocating
{
    public class PackageLocation<TPackageType>
    {
        public TPackageType PackageDetails { get; set; }
        public string PackageVersion { get; set; }
    }
}