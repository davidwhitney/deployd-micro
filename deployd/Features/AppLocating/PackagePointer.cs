namespace deployd.Features.AppLocating
{
    public class PackagePointer
    {
        public string PathAndFileName { get; set; }

        public PackagePointer(string pathAndFileName)
        {
            PathAndFileName = pathAndFileName;
        }

        public bool IsZipFile
        {
            get { return PathAndFileName.ToLower().EndsWith(".zip"); }
        }
    }
}