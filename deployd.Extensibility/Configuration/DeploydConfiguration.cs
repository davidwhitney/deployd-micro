namespace deployd.Extensibility.Configuration
{
    public class DeploydConfiguration
    {
        public PackageType PackageType { get; set; }
        public string PackageSource { get; set; }
        public string InstallRoot { get; set; }
        
        public DeploydConfiguration()
        {
            InstallRoot = "~\\Apps";
        }
    }
}
