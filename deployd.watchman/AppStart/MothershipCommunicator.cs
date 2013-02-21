using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using SimpleServices;

namespace deployd.watchman.AppStart
{
    public class MothershipCommunicator : IWindowsService 
    {
        private readonly Task _phoneHomeTask;
        private readonly CancellationTokenSource _cancellation;
        public ApplicationContext AppContext { get; set; }

        public MothershipCommunicator()
        {
            _cancellation = new CancellationTokenSource();
            _phoneHomeTask = new Task(PhoneHome, _cancellation.Token, TaskCreationOptions.LongRunning);
        }

        public void Start(string[] args)
        {
            _phoneHomeTask.Start();
        }

        public void Stop()
        {
            _cancellation.Cancel();
            _phoneHomeTask.Wait();
        }

        private void PhoneHome()
        {
            while (true)
            {
                var restClient = new RestClient("http://localhost:9001") {Timeout = 1000};
                var request = new RestRequest("/api/v1/clients/register", Method.POST) {RequestFormat = DataFormat.Json};
                request.AddBody(new { @null = string.Empty });
                restClient.Post(request);

                if (_cancellation.IsCancellationRequested)
                {
                    break;
                }

                Thread.Sleep(new TimeSpan(0,0,0, 30));
            }
        }
    }
}
