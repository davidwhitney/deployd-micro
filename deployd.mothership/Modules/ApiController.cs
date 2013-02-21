using Nancy;

namespace deployd.mothership.Modules
{
    public class ApiController : NancyModule
    {
        public ApiController() 
            : base("/api/v1")
        {
            Get["/clients"] = x =>
                {
                    return Response.AsJson("ok");
                };

            Post["/clients/register"] = x =>
                {
                    return Response.AsJson("ok");
                };
        }
    }
}