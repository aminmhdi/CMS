using System;
using Microsoft.AspNetCore.Identity;

namespace CMS.ViewModel.Settings
{
    public class AppSettings
    {
        public Logging Logging { get; set; }

        public AdminUserSeed AdminUserSeed { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public ActiveDatabase ActiveDatabase { get; set; }

        public CookieOptions CookieOptions { get; set; }

        public bool EnableEmailConfirmation { get; set; }

        public LockoutOptions LockoutOptions { get; set; }

        public PasswordOptions PasswordOptions { get; set; }

        public DataProtectionOptions DataProtectionOptions { get; set; }

        public DataProtectionX509Certificate DataProtectionX509Certificate { get; set; }

        public TimeSpan EmailConfirmationTokenProviderLifespan { get; set; }

        public int NotAllowedPreviouslyUsedPasswords { get; set; }

        public int ChangePasswordReminderDays { get; set; }

        public string[] PasswordsBanList { get; set; }

        public string[] EmailsBanList { get; set; }
    }
}
