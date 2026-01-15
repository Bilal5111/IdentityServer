using System.Web.Http;

namespace IdentityServerTest.Controllers
{
    [Authorize] 
    [RoutePrefix("api/testtoken")]
    public class TestTokenController : ApiController
    {
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
