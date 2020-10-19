using System;
using System.Linq;
using System.Threading.Tasks;
using CMS.Common.GuardToolkit;
using CMS.Common.IdentityToolkit;
using CMS.DataLayer.Context;
using CMS.ServiceLayer.Contracts.Identity;
using CMS.ServiceLayer.Contracts.Sample;
using CMS.ViewModel.Sample;
using CMS.ViewModel.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CMS.ServiceLayer.Identity
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2577
    /// And http://www.dotnettips.info/post/2578
    /// </summary>
    public class IdentityDbInitializer : IIdentityDbInitializer
    {
        private readonly IOptionsSnapshot<AppSettings> _seedOptions;
        private readonly ISampleService _sampleService;
        private readonly ILogger<IdentityDbInitializer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public IdentityDbInitializer
        (
            ISampleService sampleService,
            IServiceScopeFactory scopeFactory,
            IOptionsSnapshot<AppSettings> seedOptions,
            ILogger<IdentityDbInitializer> logger
            )
        {
            _sampleService = sampleService;
            _sampleService.CheckArgumentIsNull(nameof(_sampleService));

            _scopeFactory = scopeFactory;
            _scopeFactory.CheckArgumentIsNull(nameof(_scopeFactory));

            _seedOptions = seedOptions;
            _seedOptions.CheckArgumentIsNull(nameof(_seedOptions));

            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));
        }

        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        public void Initialize()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (_seedOptions.Value.ActiveDatabase == ActiveDatabase.InMemoryDatabase)
            {
                context.Database.EnsureCreated();
            }
            else
            {
                context.Database.Migrate();
            }
        }

        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        public void SeedData()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            var identityDbSeedData = serviceScope.ServiceProvider.GetRequiredService<IIdentityDbInitializer>();
            var result = identityDbSeedData.SeedDatabaseAsync().Result;
            if (!result)
            {
                throw new InvalidOperationException();
            }

            // How to add initial data to the DB directly
            //using var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //if (context.Roles.Any()) return;
            //context.Add(new Role(ConstantRoles.Admin));
            //context.SaveChanges();
        }

        public async Task<bool> SeedDatabaseAsync()
        {
            var sampleSeed = _seedOptions.Value.SampleSeed;
            sampleSeed.CheckArgumentIsNull(nameof(sampleSeed));

            const string thisMethodName = nameof(SeedDatabaseAsync);

            var adminUser = await _sampleService.Exists();
            if (adminUser)
            {
                _logger.LogInformation($"{thisMethodName}: Sample is already exists.");
                return true;
            }

            //Create the `Sample` if it does not exist

            var sampleResult = await _sampleService.Create(new SampleViewModel
            {
                Title = sampleSeed.Title
            });

            if (sampleResult <= 0)
            {
                _logger.LogError($"{thisMethodName}: adminRole CreateAsync failed. {sampleResult}");
                return false;
            }

            _logger.LogInformation($"{thisMethodName}: adminRole already exists.");
            return true;
        }
    }
}