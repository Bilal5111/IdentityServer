using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel.Client;
using System.Security.Claims;

namespace IdentityServerTest.Controllers
{
    [Authorize] // Web API Authorize
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("GetAccessToken")]
        public async Task<HttpResponseMessage> GetAccessToken()
        {
            var client = new HttpClient();

            var tokenResponse = await client.RequestPasswordTokenAsync(
                new PasswordTokenRequest
                {
                    Address = ConfigurationManager.AppSettings["Authority"] + "connect/token",
                    ClientId = ConfigurationManager.AppSettings["ClientID"],
                    ClientSecret = ConfigurationManager.AppSettings["ClientSecret"],
                    Scope = ConfigurationManager.AppSettings["Scope"],
                    UserName = ConfigurationManager.AppSettings["Username"],
                    Password = ConfigurationManager.AppSettings["Password"]
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

        [HttpGet]
        [Route("secure")]
        public IHttpActionResult Secure()
        {
            return Ok(new
            {
                message = "Authorized"
            });
        }
    }
}
