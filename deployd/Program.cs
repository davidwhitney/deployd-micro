using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deployd.AppStart;
using deployd.Features.ClientConfiguration;

namespace deployd
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new ApplicationContext();
            var configManager = context.Kernel.GetService<ClientConfigurationManager>();
            var configuration = configManager.LoadConfig();

        }
    }
}
