﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Entities.Identity;
using CMS.ServiceLayer.Contracts.Identity;
using CMS.ViewModel.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CMS.ServiceLayer.Identity
{
    /// <summary>
    /// Extending the Built-in Password Validation
    /// More info: http://www.dotnettips.info/post/2579
    /// </summary>
    public class CustomPasswordValidator : PasswordValidator<User>
    {
        private readonly IUsedPasswordsService _usedPasswordsService;
        private readonly ISet<string> _passwordsBanList;

        public CustomPasswordValidator
        (
            IdentityErrorDescriber errors,// How to use CustomIdentityErrorDescriber
            IOptionsSnapshot<AppSettings> appSettings,
            IUsedPasswordsService usedPasswordsService
        ) : base(errors)
        {
            _usedPasswordsService = usedPasswordsService ?? throw new ArgumentNullException(nameof(_usedPasswordsService));
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));
            if(appSettings.Value.PasswordsBanList == null) return;
            _passwordsBanList = new HashSet<string>(appSettings.Value.PasswordsBanList, StringComparer.OrdinalIgnoreCase);

            if (!_passwordsBanList.Any())
            {
                throw new InvalidOperationException("Please fill the passwords ban list in the appsettings.json file.");
            }
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            var errors = new List<IdentityError>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordIsNotSet",
                    Description = "لطفا کلمه‌ی عبور را تکمیل کنید."
                });
                return IdentityResult.Failed(errors.ToArray());
            }

            if (string.IsNullOrWhiteSpace(user?.UserName))
            {
                errors.Add(new IdentityError
                {
                    Code = "UserNameIsNotSet",
                    Description = "لطفا نام کاربری را تکمیل کنید."
                });
                return IdentityResult.Failed(errors.ToArray());
            }

            // First use the built-in validator
            var result = await base.ValidateAsync(manager, user, password);
            errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            // Extending the built-in validator
            if (password.Contains(user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsUserName",
                    Description = "کلمه‌ی عبور نمی‌تواند حاوی قسمتی از نام کاربری باشد."
                });
                return IdentityResult.Failed(errors.ToArray());
            }

            if (!IsSafePasword(password))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordIsNotSafe",
                    Description = "کلمه‌ی عبور وارد شده به سادگی قابل حدس زدن است."
                });
                return IdentityResult.Failed(errors.ToArray());
            }

            if (await _usedPasswordsService.IsPreviouslyUsedPasswordAsync(user, password))
            {
                errors.Add(new IdentityError
                {
                    Code = "IsPreviouslyUsedPassword",
                    Description = "لطفا کلمه‌ی عبور دیگری را انتخاب کنید. این کلمه‌ی عبور پیشتر توسط شما استفاده شده‌است و تکراری می‌باشد."
                });
                return IdentityResult.Failed(errors.ToArray());
            }

            return !errors.Any() ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }

        private static bool AreAllCharsEqual(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return false;
            data = data.ToLowerInvariant();
            var firstElement = data.ElementAt(0);
            var equalCharsLen = data.ToCharArray().Count(x => x == firstElement);
            if (equalCharsLen == data.Length) return true;
            return false;
        }

        private bool IsSafePasword(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return false;
            if (data.Length < 5) return false;
            if (_passwordsBanList.Contains(data.ToLowerInvariant())) return false;
            if (AreAllCharsEqual(data)) return false;

            return true;
        }
    }
}