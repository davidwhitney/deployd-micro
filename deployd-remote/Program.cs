using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using RestSharp;

namespace deployd_remote
{
    class Program
    {
        static void Main(string[] args)
        {
            var appName = string.Empty;
            var hostName = string.Empty;
            var help = false;

            var p = new OptionSet
                {
                    {"app=", v => appName = v},
                    {"host=", v => hostName = v},
                    {"h|?|help", v => help = v != null},
                };
            p.Parse(args);

            if (help || string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(hostName))
            {
                p.WriteOptionDescriptions(Console.Out);
                return;
            }

            var restClient = new RestClient("http://" + hostName + ":9000");
            var request = new RestRequest("/api/v1/" + appName, Method.PUT).AddBody(new {});
            var response = restClient.Put(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                Console.WriteLine("'{0}' deployment started remotely on '{1}'.", appName, hostName);
            }

        }
    }
}
