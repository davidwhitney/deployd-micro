using System;

namespace deployd.Features.AppInstallation.Hooks
{
    public class Hook
    {
        public string FileName { get; set; }
        public HookType Type { get; set; }
        public Type Class { get; set; }

        public Hook(string fileName, HookType type)
        {
            FileName = fileName;
            Type = type;
        }

        public Hook(Type @class)
        {
            Class = @class;
            Type = HookType.Class;
        }
    }
}