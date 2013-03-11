namespace deployd_package.Features.MetadataDiscovery
{
    public interface IConventionsSettingsFileLoader
    {
        ConventionsSettingsFile Load(string fileName);
    }

    public class ConventionsSettingsFileLoader : IConventionsSettingsFileLoader
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;

        public ConventionsSettingsFileLoader(System.IO.Abstractions.IFileSystem fs)
        {
            _fs = fs;
        }

        public ConventionsSettingsFile Load(string fileName)
        {
            return new ConventionsSettingsFile();
        }
    }
}