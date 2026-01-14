using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using IdentityModel;
using IdentityModel.Client;
using IdentityServerTest.Models;
using AllowAnonymousAttribute = System.Web.Http.AllowAnonymousAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace IdentityServerTest.Controllers
{
    public class ValuesController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("GetAccessToken")]
        public async Task<HttpResponseMessage> GetAccessToken()
        {
            var appsettings = GetAppSettings();

            var client = new HttpClient();

            var tokenResponse = await client.RequestPasswordTokenAsync(
                new PasswordTokenRequest
                {
                    Address = appsettings.Authority + "connect/token",
                    ClientId = appsettings.ClientID,
                    ClientSecret = appsettings.ClientSecret,
                    Scope = appsettings.Scope,
                    UserName = appsettings.Username,
                    Password = appsettings.Password                    
                });

            if (tokenResponse.IsError)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = false,
                    error = tokenResponse.Error,
                    error_description = tokenResponse.ErrorDescription
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                success = true,
                access_token = tokenResponse.AccessToken,
                expires_in = tokenResponse.ExpiresIn,
                refresh_token = tokenResponse.RefreshToken
            });
        }






        public AppSettings GetAppSettings()
        {
            return new AppSettings
            {
                Authority = ConfigurationManager.AppSettings["Authority"],
                ClientID = ConfigurationManager.AppSettings["ClientID"],
                ClientSecret = ConfigurationManager.AppSettings["ClientSecret"],
                Scope = ConfigurationManager.AppSettings["Scope"],
                Username = ConfigurationManager.AppSettings["Username"],
                Password = ConfigurationManager.AppSettings["Password"]

            };
        }
    }

}