using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.ServiceProcess;
using SimpleServices;
using deployd.watchman.AppStart;
using ServiceInstaller = System.ServiceProcess.ServiceInstaller;

namespace deployd.watchman
{
    [RunInstaller(true)]
    public class Program : ServiceInstaller
    {
        private static void Main(string[] args)
        {
            var services = new List<IWindowsService> {new NancyUi()};

            if (ConfigurationManager.AppSettings["mothership:Enabled"].ToLower() == "true")
            {
                services.Add(new MothershipCommunicator());
            }

            new Service(args, services.ToArray,
                        installationSettings: (serviceInstaller, serviceProcessInstaller) =>
                            {
                                serviceInstaller.ServiceName = "deployd.watchman";
                                serviceInstaller.StartType = ServiceStartMode.Automatic;
                                serviceProcessInstaller.Account = ServiceAccount.NetworkService;
                            },
                        configureContext: x => { x.Log = Console.WriteLine; })
                .Host();
        }
    }
}
