using System;
using System.Threading.Tasks;
using CMS.ServiceLayer.Contracts.Identity;
using CMS.ServiceLayer.Contracts.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CMS.Controllers
{
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IApplicationUserManager _userService;

        public UserController
        (
            ISharedResource sharedResource,
            ILogger<UserController> logger,
            IApplicationUserManager userService
        ) : base(sharedResource)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get(int? id)
        {
            try
            {
                if (!id.HasValue) return BadRequest(SharedResource.GetString("BadRequest"));

                var user = await _userService.Users.FirstOrDefaultAsync(q=>q.Id == id.Value);

                if (user != null)
                    return Ok(new {user.UserName});

                return NotFound(SharedResource.GetString("NotFound"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

        }
    }
}
