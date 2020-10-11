using System.Linq;
using CMS.ServiceLayer.Contracts.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Controllers
{
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        public readonly ISharedResource SharedResource;

        public BaseController(ISharedResource sharedResource)
        {
            SharedResource = sharedResource;
        }

        protected string GetHeaderValue(HttpContext httpContext, string key)
        {
            return !httpContext.Request.Headers.TryGetValue(key, out var keys) ? null : keys.First();
        }
    }
}
