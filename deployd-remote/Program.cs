using System;
using System.Net;
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
            var request = new RestRequest("/api/v1/install-queue/" + appName, Method.POST) {RequestFormat = DataFormat.Json};
            request.AddBody(new { @null = string.Empty });
            var response = restClient.Post(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                Console.WriteLine("'{0}' deployment started remotely on '{1}'.", appName, hostName);
            }

        }
    }
}
