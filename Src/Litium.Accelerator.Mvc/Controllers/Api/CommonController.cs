using Litium.Accelerator.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Litium.Accelerator.Mvc.Controllers.Api
{
    [RoutePrefix("api/common")]
    public class CommonController : ApiControllerBase
    {
        [HttpGet]
        [Route("cookieinfo")]
        public IHttpActionResult Get()
        {
            return Ok(new { name = CookieNotificationMessage.CookieName, expires = CookieNotificationMessage.Expires });
        }
    }
}
