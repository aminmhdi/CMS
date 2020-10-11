using System;
using System.Threading.Tasks;
using CMS.Entities.Identity;
using CMS.ServiceLayer.Contracts.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CMS.ServiceLayer.Identity
{
    /// <summary>
    /// Keep track of on-line users
    /// </summary>
    public class CustomSecurityStampValidator : SecurityStampValidator<User>
    {
        private readonly IOptions<SecurityStampValidatorOptions> _options;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IUserStatService _siteStatService;
        private readonly ISystemClock _clock;

        public CustomSecurityStampValidator
        (
            IOptions<SecurityStampValidatorOptions> options,
            IApplicationSignInManager signInManager,
            ISystemClock clock,
            IUserStatService siteStatService,
            ILoggerFactory logger
        ) : base(options, (SignInManager<User>)signInManager, clock, logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(_options));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(_signInManager));
            _siteStatService = siteStatService ?? throw new ArgumentNullException(nameof(_siteStatService));
            _clock = clock;
        }

        public TimeSpan UpdateLastModifiedDate { get; set; } = TimeSpan.FromMinutes(2);

        public override async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            await base.ValidateAsync(context);
            await UpdateUserLastVisitDateTimeAsync(context);
        }

        private async Task UpdateUserLastVisitDateTimeAsync(CookieValidatePrincipalContext context)
        {
            var currentUtc = DateTimeOffset.UtcNow;
            if (context.Options != null && _clock != null)
            {
                currentUtc = _clock.UtcNow;
            }
            var issuedUtc = context.Properties.IssuedUtc;

            // Only validate if enough time has elapsed
            if (issuedUtc == null || context.Principal == null)
            {
                return;
            }

            var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
            if (timeElapsed > UpdateLastModifiedDate)
            {
                await _siteStatService.UpdateUserLastVisitDateTimeAsync(context.Principal);
            }
        }
    }
}