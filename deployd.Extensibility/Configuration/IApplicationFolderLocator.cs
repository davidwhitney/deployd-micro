using System;

namespace deployd.Extensibility.Configuration
{
    public interface IApplicationFolderLocator
    {
        string ApplicationFolder { get; }
    }

    public class ApplicationFolderLocator : IApplicationFolderLocator
    {
        public string ApplicationFolder
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
    }
}