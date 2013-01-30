using System.Diagnostics;

namespace deployd.watchman.Services
{
    public class InstallationService
    {
        public void InstallPackage(string appName)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "deployd.exe",
                Arguments = string.Format("-install -app=\"{0}\"", appName),
            };

            Process.Start(startInfo);
        }
    }
}
