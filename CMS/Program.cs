using CMS.IocConfig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.InitializeDb();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.ClearProviders();
                        logging.AddDebug();

                        if (hostingContext.HostingEnvironment.IsDevelopment())
                            logging.AddConsole();

                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    }).UseStartup<Startup>();
                });
    }
}
