using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(IdentityServerTest.Startup))]

namespace IdentityServerTest
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var issuer = ConfigurationManager.AppSettings["Authority"].TrimEnd('/');
            var clientId = ConfigurationManager.AppSettings["ClientID"];

            var httpClient = new HttpClient();
            var disco = httpClient.GetDiscoveryDocumentAsync(issuer).Result;

            if (disco.IsError)
                throw new Exception(disco.Error);

            var jwksJson = httpClient.GetStringAsync(disco.JwksUri).Result;
            var keys = new JsonWebKeySet(jwksJson);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,

                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudiences = new[]
                    {
                        "api",                    // matches your token "aud"
                        issuer + "/resources"     // matches your token "aud"
                    },

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = keys.Keys,

                    NameClaimType = "sub",
                    RoleClaimType = "role"
                }
            });

            app.Use(async (context, next) =>
            {
                if (context.Authentication.User?.Identity?.IsAuthenticated == true)
                {
                    context.Request.User = context.Authentication.User;
                }
                await next();
            });

            app.UseWebApi(config);
        }
    }
}
