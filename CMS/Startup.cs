using CMS.Common.ModelBinder;
using CMS.Hubs;
using CMS.IocConfig;
using CMS.Middlewares;
using CMS.Swagger;
using CMS.ViewModel.Settings;
using DNTCommon.Web.Core;
using DNTScheduler.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CMS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(options => Configuration.Bind(options));

            services.AddCustomIdentityServices();

            services.AddLanguageServices();

            services.AddMvc(options =>
            {
                options.UseYeKeModelBinder();
                options.UsePersianDateModelBinder();
                options.UseCommaSeparatorModelBinder();
                //options.AllowEmptyInputInBodyModelBinding = true
            });

            services.AddSignalR();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS api", Version = "v1" });
                c.OperationFilter<AddHeaderParameters>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
                app.UseExceptionHandler("/error/index/500");
                app.UseStatusCodePagesWithReExecute("/error/index/{0}");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS API"); });

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder => { appBuilder.UseLanguage(); });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDNTScheduler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute
                (
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapRazorPages();

                endpoints.MapHub<MessageHub>("/message");
            });
        }
    }
}
