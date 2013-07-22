using System;
using System.Collections.Generic;
using System.Linq;
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
            var environment=string.Empty;

            bool prepare=false;
            bool force=false;
            var p = new OptionSet
                {
                    {"app=", v => appName = v},
                    {"host=", v => hostName = v},
                    {"e|environment=", v => environment = v},
                    {"p|prep", v => prepare = v != null},
                    {"f|force", v => force = v != null},
                    {"h|?|help", v => help = v != null},
                };
            p.Parse(args);

            if (help || string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(hostName))
            {
                p.WriteOptionDescriptions(Console.Out);
                return;
            }

            var restClient = new RestClient("http://" + hostName + ":9000");
            var requestUrl = "/api/v1/install-queue/" + appName;
            var query = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(environment))
            {
                query.Add("environment", environment);
            }

            if (prepare)
            {
                query.Add("prepareOnly", "true");
            }

            if (force)
            {
                query.Add("forceDownload", "true");
            }

            requestUrl += "?" + string.Join("&", query.Select(q => q.Key + "=" + q.Value).ToArray());

            var request = new RestRequest(requestUrl, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(new { @null = string.Empty });
            var response = restClient.Post(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                Console.WriteLine("'{0}' deployment started remotely on '{1}'.", appName, hostName);
            }

        }
    }
}
