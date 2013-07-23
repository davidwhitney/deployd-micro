using System;

namespace deployd.Features.AppInstallation.Hooks
{
    public class HookTypeRef
    {
        public string FileName { get; set; }
        public HookType Type { get; set; }
        public Type Class { get; set; }

        public HookTypeRef(string fileName, HookType type)
        {
            FileName = fileName;
            Type = type;
        }

        public HookTypeRef(Type @class)
        {
            Class = @class;
            Type = HookType.Class;
        }
    }
}