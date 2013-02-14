using Nancy;

namespace deployd.mothership.Modules
{
    public class HomeController : NancyModule
    {
        public HomeController()
        {
            Get["/"] = x => Response.AsText("deployd watchman");
        }
    }
}
