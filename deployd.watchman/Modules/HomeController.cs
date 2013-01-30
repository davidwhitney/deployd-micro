using Nancy;

namespace deployd.watchman.Modules
{
    public class HomeController : NancyModule
    {
        public HomeController()
        {
            Get["/"] = x =>
            {
                
                return View["index.cshtml"];
            };
        }
    }
}
