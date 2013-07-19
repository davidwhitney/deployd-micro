using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using SimpleServices;
using deployd.watchman.AppStart;
using log4net;
using ServiceInstaller = System.ServiceProcess.ServiceInstaller;

namespace deployd.watchman
{
    [RunInstaller(true)]
    public class Program : ServiceInstaller
    {
        private static void Main(string[] args)
        {
            var config = log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            var logger = LogManager.GetLogger(typeof (Program));
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
