using System;
using System.Threading.Tasks;
using CMS.DataLayer.Context;
using CMS.IocConfig;
using CMS.ServiceLayer.Contracts.Sample;
using CMS.ViewModel.Sample;
using CMS.ViewModel.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using NUnit.Framework;

namespace CMS.UnitTest
{
    [TestFixture]
    public class Tests
    {
        private ISampleService _sampleService;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder();
            configuration.Add(new JsonConfigurationSource
            {
                FileProvider = new PhysicalFileProvider("D:\\Projects\\CMS\\CMS\\"), 
                Optional = true,
                Path = "appsettings.json", 
                ReloadDelay = 250, 
                ReloadOnChange = true, 
                OnLoadException = null
            });

            services.Configure<AppSettings>(options => configuration.Build().Bind(options));

            services.GetSiteSettings();
            services.AddCustomIdentityServices();

            services.AddLanguageServices();

            _sampleService = ServiceLocator.Current.GetInstance<ISampleService>();
        }

        [Test]
        public async Task ListTest()
        {
            try
            {
                var search = new SampleSearchViewModel();
                var result = await _sampleService.List(search);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.Pass();
        }
    }
}