using System.Collections.Generic;

namespace deployd.Features.AppInstallation.Hooks
{
    public class Hooks
    {
        public List<Hook> FirstInstall { get; set; }
        public List<Hook> PreInstall { get; set; }
        public List<Hook> PostInstall { get; set; }

        public Hooks()
        {
            FirstInstall = new List<Hook>();
            PreInstall = new List<Hook>();
            PostInstall = new List<Hook>();
        }
    }
}