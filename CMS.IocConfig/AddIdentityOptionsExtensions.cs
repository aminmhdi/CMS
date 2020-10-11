using System;
using System.Threading.Tasks;
using CMS.Entities.Identity;
using CMS.ServiceLayer.Identity;
using CMS.ViewModel.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.IocConfig
{
    public static class AddIdentityOptionsExtensions
    {
        public const string EmailConfirmationTokenProviderName = "ConfirmEmail";

        public static IServiceCollection AddIdentityOptions
        (
            this IServiceCollection services, 
            AppSettings appSettings)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));

            services.AddConfirmEmailDataProtectorTokenOptions(appSettings);
            services.AddIdentity<User, Role>(identityOptions =>
            {
                SetPasswordOptions(identityOptions.Password, appSettings);
                SetSignInOptions(identityOptions.SignIn, appSettings);
                SetUserOptions(identityOptions.User);
                SetLockoutOptions(identityOptions.Lockout, appSettings);
            }).AddUserStore<ApplicationUserStore>()
              .AddUserManager<ApplicationUserManager>()
              .AddRoleStore<ApplicationRoleStore>()
              .AddRoleManager<ApplicationRoleManager>()
              .AddSignInManager<ApplicationSignInManager>()
              .AddErrorDescriber<CustomIdentityErrorDescriber>()
              // You **cannot** use .AddEntityFrameworkStores() when you customize everything
              //.AddEntityFrameworkStores<ApplicationDbContext, int>()
              .AddDefaultTokenProviders()
              .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<User>>(EmailConfirmationTokenProviderName);

            services.ConfigureApplicationCookie(identityOptionsCookies =>
            {
                var provider = services.BuildServiceProvider();
                SetApplicationCookieOptions(provider, identityOptionsCookies, appSettings);
            });

            services.EnableImmediateLogout();

            return services;
        }

        private static void AddConfirmEmailDataProtectorTokenOptions(this IServiceCollection services, AppSettings appSettings)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderName;
            });

            services.Configure<ConfirmEmailDataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = appSettings.EmailConfirmationTokenProviderLifespan;
            });
        }

        private static void EnableImmediateLogout(this IServiceCollection services)
        {
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                options.ValidationInterval = TimeSpan.Zero;
                options.OnRefreshingPrincipal = principalContext => Task.CompletedTask;
            });
        }

        private static void SetApplicationCookieOptions(IServiceProvider provider, CookieAuthenticationOptions identityOptionsCookies, AppSettings appSettings)
        {
            identityOptionsCookies.Cookie.Name = appSettings.CookieOptions.CookieName;
            identityOptionsCookies.Cookie.HttpOnly = true;
            identityOptionsCookies.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            identityOptionsCookies.Cookie.SameSite = SameSiteMode.Lax;
            identityOptionsCookies.Cookie.IsEssential = true; //  this cookie will always be stored regardless of the user's consent

            identityOptionsCookies.ExpireTimeSpan = appSettings.CookieOptions.ExpireTimeSpan;
            identityOptionsCookies.SlidingExpiration = appSettings.CookieOptions.SlidingExpiration;
            identityOptionsCookies.LoginPath = appSettings.CookieOptions.LoginPath;
            identityOptionsCookies.LogoutPath = appSettings.CookieOptions.LogoutPath;
            identityOptionsCookies.AccessDeniedPath = appSettings.CookieOptions.AccessDeniedPath;

            if (appSettings.CookieOptions.UseDistributedCacheTicketStore)
            {
                // To manage large identity cookies
                //identityOptionsCookies.SessionStore = provider.GetRequiredService<ITicketStore>();
            }
        }

        private static void SetLockoutOptions(LockoutOptions identityOptionsLockout, AppSettings appSettings)
        {
            identityOptionsLockout.AllowedForNewUsers = appSettings.LockoutOptions.AllowedForNewUsers;
            identityOptionsLockout.DefaultLockoutTimeSpan = appSettings.LockoutOptions.DefaultLockoutTimeSpan;
            identityOptionsLockout.MaxFailedAccessAttempts = appSettings.LockoutOptions.MaxFailedAccessAttempts;
        }

        private static void SetPasswordOptions(PasswordOptions identityOptionsPassword, AppSettings appSettings)
        {
            identityOptionsPassword.RequireDigit = appSettings.PasswordOptions.RequireDigit;
            identityOptionsPassword.RequireLowercase = appSettings.PasswordOptions.RequireLowercase;
            identityOptionsPassword.RequireNonAlphanumeric = appSettings.PasswordOptions.RequireNonAlphanumeric;
            identityOptionsPassword.RequireUppercase = appSettings.PasswordOptions.RequireUppercase;
            identityOptionsPassword.RequiredLength = appSettings.PasswordOptions.RequiredLength;
        }

        private static void SetSignInOptions(SignInOptions identityOptionsSignIn, AppSettings appSettings)
        {
            identityOptionsSignIn.RequireConfirmedEmail = appSettings.EnableEmailConfirmation;
        }

        private static void SetUserOptions(UserOptions identityOptionsUser)
        {
            identityOptionsUser.RequireUniqueEmail = true;
        }
    }
}