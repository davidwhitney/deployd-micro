using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceProcess;
using SimpleServices;
using deployd.mothership.AppStart;
using ServiceInstaller = System.ServiceProcess.ServiceInstaller;

namespace deployd.mothership
{
    [RunInstaller(true)]
    public class Program : ServiceInstaller
    {
        private static void Main(string[] args)
        {
            new Service(args,
                        new List<IWindowsService>{ new NancyUi() }.ToArray,
                        installationSettings: (serviceInstaller, serviceProcessInstaller) =>
                            {
                                serviceInstaller.ServiceName = "deployd.mothership";
                                serviceInstaller.StartType = ServiceStartMode.Automatic;
                                serviceProcessInstaller.Account = ServiceAccount.NetworkService;
                            },
                        configureContext: x => { x.Log = Console.WriteLine; })
                .Host();
        }
    }
}
