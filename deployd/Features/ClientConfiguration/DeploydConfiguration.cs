namespace deployd.Features.ClientConfiguration
{
    public class DeploydConfiguration
    {
        public PackageType PackageType { get; set; }
        public string PackageSource { get; set; }
        public string InstallRoot { get; set; }
        
        public bool DefaultConfiguration { get; set; }

        public DeploydConfiguration()
        {
            InstallRoot = "~\\Apps";
        }
    }
}
