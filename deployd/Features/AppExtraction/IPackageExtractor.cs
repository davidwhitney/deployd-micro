namespace deployd.Features.AppExtraction
{
    public interface IPackageExtractor
    {
        bool CanUnpack(object packageInfo);
        void Unpack(string targetDirectory, object packageInfo);
    }
}
