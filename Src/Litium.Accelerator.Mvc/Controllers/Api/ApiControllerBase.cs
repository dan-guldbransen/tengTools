using Litium.Web.WebApi;
using System.Web.Http;

namespace Litium.Accelerator.Mvc.Controllers.Api
{
    /// <summary>
    /// Api controller base class.
    /// </summary>
    [ApiCollection("site")]
    public abstract class ApiControllerBase : ApiController
    {
    }
}