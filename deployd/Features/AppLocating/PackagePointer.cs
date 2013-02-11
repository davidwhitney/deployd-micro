namespace deployd.Features.AppLocating
{
    public class PackagePointer
    {
        public string PathAndFileName { get; set; }

        public bool IsZipFile
        {
            get { return PathAndFileName.ToLower().EndsWith(".zip"); }
        }
    }
}