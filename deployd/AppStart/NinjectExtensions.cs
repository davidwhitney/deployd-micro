using Ninject;

namespace deployd.AppStart
{
    public static class NinjectExtensions
    {
        public static TType GetService<TType>(this IKernel krnl)
        {
            return (TType) krnl.GetService(typeof (TType));
        }
    }
}