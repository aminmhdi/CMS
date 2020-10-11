﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CMS.Common.WebToolkit;
using CMS.ServiceLayer.Identity;
using CMS.ViewModel.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CMS.IocConfig
{
    public static class CustomDataProtectionExtensions
    {
        public static IServiceCollection AddCustomDataProtection
        (
            this IServiceCollection services,
            AppSettings appSettings
        )
        {
            services.AddSingleton<IXmlRepository, DataProtectionKeyService>();
            services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(serviceProvider =>
            {
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                    using var scope = scopeFactory.CreateScope();
                    options.XmlRepository = scope.ServiceProvider.GetRequiredService<IXmlRepository>();
                });
            });

            //var certificate = loadCertificateFromFile(siteSettings);
            services
                .AddDataProtection()
                .SetDefaultKeyLifetime(appSettings.DataProtectionOptions.DataProtectionKeyLifetime)
                .SetApplicationName(appSettings.DataProtectionOptions.ApplicationName);
            //.ProtectKeysWithCertificate(certificate);

            return services;
        }

        private static X509Certificate2 LoadCertificateFromFile(AppSettings siteSettings)
        {
            // NOTE:
            // You should check out the identity of your application pool and make sure
            // that the `Load user profile` option is turned on, otherwise the crypto susbsystem won't work.

            var certificate = siteSettings.DataProtectionX509Certificate;
            var fileName = Path.Combine(ServerInfo.GetAppDataFolderPath(), certificate.FileName);

            // For decryption the certificate must be in the certificate store. It's a limitation of how EncryptedXml works.
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(new X509Certificate2(fileName, certificate.Password, X509KeyStorageFlags.Exportable));
            }

            var cert = new X509Certificate2
            (
                fileName,
                certificate.Password,
                keyStorageFlags: X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            // TODO: If you are getting `Keyset does not exist`, run `wwwroot\App_Data\make-cert.cmd` again.
            Console.WriteLine($"cert private key: {cert.PrivateKey}");
            return cert;
        }
    }
}