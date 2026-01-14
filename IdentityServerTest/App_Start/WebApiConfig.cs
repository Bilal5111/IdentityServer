using System;
using System.Web.Http;
using Swashbuckle.Application;
using System.IO;

namespace IdentityServerTest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "IdentityServerTest API");

                try
                {
                    var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\IdentityServerTest.xml");
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
                catch
                {
                    // Ignore if file missing or cannot read — Swagger will still work
                }
            })
            .EnableSwaggerUi();
        }
    }
}
