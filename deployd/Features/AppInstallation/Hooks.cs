using System.Collections.Generic;

namespace deployd.Features.AppInstallation
{
    public class Hooks
    {
        public List<string> FirstInstall { get; set; }
        public List<string> PreInstall { get; set; }
        public List<string> PostInstall { get; set; }

        public Hooks()
        {
            FirstInstall = new List<string>();
            PreInstall = new List<string>();
            PostInstall = new List<string>();
        }
    }
}