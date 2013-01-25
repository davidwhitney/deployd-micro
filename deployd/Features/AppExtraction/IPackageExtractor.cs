namespace deployd.Features.AppExtraction
{
    public interface IPackageExtractor
    {
        void Unpack(string targetDirectory, object packageInfo);
    }
}
