using Microsoft.Owin;
using Owin;

using System.Web.Http;

[assembly: OwinStartup(typeof(tcp_server.Starup))]

namespace tcp_server
{
    class Starup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}
