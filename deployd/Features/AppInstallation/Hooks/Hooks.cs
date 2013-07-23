using System.Collections.Generic;

namespace deployd.Features.AppInstallation.Hooks
{
    public class Hooks
    {
        public List<HookTypeRef> FirstInstall { get; set; }
        public List<HookTypeRef> PreInstall { get; set; }
        public List<HookTypeRef> PostInstall { get; set; }
        public List<HookTypeRef> PostFirstInstall { get; set; }

        public Hooks()
        {
            FirstInstall = new List<HookTypeRef>();
            PreInstall = new List<HookTypeRef>();
            PostInstall = new List<HookTypeRef>();
            PostFirstInstall = new List<HookTypeRef>();
        }
    }
}