namespace deployd.mothership.AppStart
{
    public class NancyConventionsBootstrapper : Nancy.Bootstrappers.Ninject.NinjectNancyBootstrapper
    {
        protected override void ApplicationStartup(Ninject.IKernel container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("../../Views/", context.ModuleName.Replace("Controller", ""), "/", viewName));
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("Views/", context.ModuleName.Replace("Controller", ""), "/", viewName));
        } 
    }
}
