using System;

namespace deployd.Features.AppLocating
{
    public class NoPackageFoundException : Exception
    {
        private readonly string _packageName;

        public override string Message
        {
            get { return "No package matching " + _packageName + " found."; }
        }

        public NoPackageFoundException(string packageName)
        {
            _packageName = packageName;
        }
    }
}