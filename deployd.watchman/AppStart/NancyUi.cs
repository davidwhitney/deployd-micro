using System;
using Nancy.Hosting.Self;
using SimpleServices;

namespace deployd.watchman.AppStart
{
    public class NancyUi : IWindowsService
    {
        public ApplicationContext AppContext { get; set; }
        private readonly NancyHost _host;

        public NancyUi()
        {
            _host = new NancyHost(new Uri("http://localhost:9000"));
        }

        public void Start(string[] args)
        {
            _host.Start();
        }

        public void Stop()
        {
            _host.Stop();
        }
    }
}